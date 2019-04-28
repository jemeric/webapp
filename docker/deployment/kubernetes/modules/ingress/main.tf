# Setup NGNIX ingress controller with Heml

# create Tiller service account
resource "kubernetes_service_account" "tiller_service" {
  metadata {
    name      = "terraform-tiller"
    namespace = "kube-system"
  }

  automount_service_account_token = true
}

resource "kubernetes_cluster_role_binding" "tiller_sercice_roll" {
  depends_on = ["kubernetes_service_account.tiller_service"]

  metadata {
    name = "terraform-tiller"
  }

  role_ref {
    api_group = "rbac.authorization.k8s.io"
    name      = "cluster-admin"
    kind      = "ClusterRole"
  }

  subject {
    kind      = "ServiceAccount"
    name      = "${kubernetes_service_account.tiller_service.metadata.0.name}"
    namespace = "kube-system"
  }
}

# bind Tiller serviceaccount to cluster-admin role

# setup nginx ingress - creats load balancer/ingress controller
resource "helm_release" "nginx_ingress" {
  depends_on = ["kubernetes_cluster_role_binding.tiller_sercice_roll"]
  name       = "webappload-balancer"
  chart      = "stable/nginx-ingress"
}

data "kubernetes_service" "webapp_loadbalancer" {
  depends_on = ["helm_release.nginx_ingress"]

  metadata {
    name = "webappload-balancer-nginx-ingress-controller"
  }
}

resource "digitalocean_domain" "default" {
  depends_on = ["helm_release.nginx_ingress"]
  name       = "${var.cluster_domain}"
  ip_address = "${data.kubernetes_service.webapp_loadbalancer.load_balancer_ingress.0.ip}"
}

resource "digitalocean_record" "www" {
  domain = "${digitalocean_domain.default.name}"
  type   = "A"
  name   = "www"
  value  = "${data.kubernetes_service.webapp_loadbalancer.load_balancer_ingress.0.ip}"
}

# Add MX records so we can use @domain email addresses for let's encrypt
# TODO - would be nice to include this using dynamic resource for_each on variable but it doesn't seem to be supported eyt
resource "digitalocean_record" "mx1" {
  domain   = "${digitalocean_domain.default.name}"
  type     = "MX"
  name     = "@"
  value    = "mx1.improvmx.com."
  priority = 10
}

resource "digitalocean_record" "mx2" {
  domain   = "${digitalocean_domain.default.name}"
  type     = "MX"
  name     = "@"
  value    = "mx2.improvmx.com."
  priority = 20
}

# shouldn't need to do this when terraform adds support: https://github.com/terraform-providers/terraform-provider-kubernetes/issues/14
# An ingress resource is what Kubernetes uses to expose the service outside the cluster
data "template_file" "ingress" {
  template = "${file("templates/ingress.yaml.tpl")}"

  vars {
    INGRESS_NAME   = "webapp-ingress"
    INCLUDE_ISSUER = false
    ISSUER_NAME    = "${var.issuer_name}"
    HOSTNAME       = "${var.cluster_domain}"
    SERVICENAME    = "${var.service_name}"
    SERVICEPORT    = "${var.service_port}"
  }
}

resource "null_resource" "ingress" {
  depends_on = ["helm_release.nginx_ingress", "digitalocean_domain.default", "digitalocean_record.mx1", "digitalocean_record.mx2"]

  triggers = {
    manifest_sha1 = "${sha1("${data.template_file.ingress.rendered}")}"
  }

  # NOTE - when you add a domain you - accessing from the load balancer IP address directly will return a 404
  provisioner "local-exec" {
    #command = "kubectl --kubeconfig ${path.module}/.kubeconfig apply -f -<<EOF\n${data.template_file.ingress.rendered}\nEOF"
    # Run "Set-Service ssh-agent -StartupType Manual" before using Powershell
    command = "@'\n${data.template_file.ingress.rendered}\n'@ | kubectl --kubeconfig ${var.kube_config_path} apply -f -"

    interpreter = ["PowerShell", "-Command"]
  }

  provisioner "local-exec" {
    when    = "destroy"
    command = "echo 'Ingress config to be destroyed'"
  }
}

# This is just here to force the certificate setup to wait on ingress (won't be needed)
resource "null_resource" "noop" {
  depends_on = ["null_resource.ingress"]
}
