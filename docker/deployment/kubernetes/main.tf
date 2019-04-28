# provider blocked used named provider (i.e. digitalocean, aws) - provider is responsible for creating/managing resources
# use environment variable DIGITALOCEAN_TOKEN to provide access token (https://www.terraform.io/docs/providers/do/index.html#token)
provider "digitalocean" {}

# Create a Kubernetes Cluster (creates nodes)
# In Terraform second resource string declare name used to refer to this resource from elsewhere in Terraform module
# it has no significance outside the scope of the module
resource "digitalocean_kubernetes_cluster" "cluster" {
  name    = "${var.cluster-name}"
  region  = "${var.cluster-region}"
  version = "${var.cluster-version}"
  tags    = ["${var.cluster-name}"]

  node_pool {
    name       = "${var.cluster-node-pool-name}"
    size       = "${var.cluster-node-pool-size}"
    node_count = "${var.cluster-node-pool-node-count}"
  }
}

# Setup Kubernetes Provider
provider "kubernetes" {
  host = "${digitalocean_kubernetes_cluster.cluster.endpoint}"

  client_certificate     = "${base64decode(digitalocean_kubernetes_cluster.cluster.kube_config.0.client_certificate)}"
  client_key             = "${base64decode(digitalocean_kubernetes_cluster.cluster.kube_config.0.client_key)}"
  cluster_ca_certificate = "${base64decode(digitalocean_kubernetes_cluster.cluster.kube_config.0.cluster_ca_certificate)}"
}

# Export Kubeconfig - creates local file .kubeconfig for kubectl
resource "local_file" "kubeconfig" {
  content  = "${digitalocean_kubernetes_cluster.cluster.kube_config.0.raw_config}"
  filename = "${path.module}/.kubeconfig"
}

# gives so-called virtual IP (cluster IP) for the pods that have a certain label
# and those pods are basically app containers you deployed with deployment your 
# app's container with some specifications (gives pods a stable IP)
resource "kubernetes_service" "webapp_service" {
  metadata {
    name = "webapp-service"
  }

  spec {
    selector {
      app = "${kubernetes_deployment.webapp_deployment.metadata.0.labels.app}"
    }

    port {
      protocol    = "TCP"
      port        = "80"
      target_port = 5001  # targets TCP port 5001 on any pod with "app={app_label}"
    }
  }
}

# create docker secret so we can download from private repository
resource "kubernetes_secret" "webapp_secret" {
  metadata {
    name = "docker-cfg"
  }

  data {
    # credStore's currently not supported - may need to rebuild this file (https://stackoverflow.com/a/47003541)
    ".dockerconfigjson" = "${file(pathexpand("~/.docker/config.json"))}"
  }
  
  type = "kubernetes.io/dockerconfigjson"
}

# manages pods in nodes
resource "kubernetes_deployment" "webapp_deployment" {
  metadata {
    name = "webapp-deployment"

    labels {
      app = "webapp"
    }
  }

  spec {
    replicas = 3 # create 3 replicated pods

    selector {
      # defines how the deployment finds which pods to manage (select label that is defined in Pod template, app: webapp)
      match_labels {
        app = "webapp"
      }
    }

    template {
      metadata {
        labels {
          app = "webapp"
        }
      }

      spec {
        container {
          # indicates containers Pods run in
          #image = "docker.io/jericmason/webapp:prod"
          image = "docker.io/jericmason/production:private"
          name  = "webapp"

          port {
            container_port = 5001 # open port so container can send/accept traffic
          }
        }
        image_pull_secrets {
          name = "${kubernetes_secret.webapp_secret.metadata.0.name}"
        }
      }
    }
  }
}

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
    name = "cluster-admin"
    kind = "ClusterRole"
  }

  subject {
    kind      = "ServiceAccount"
    name      = "${kubernetes_service_account.tiller_service.metadata.0.name}"
    namespace = "kube-system"
  }
}

# bind Tiller serviceaccount to cluster-admin role

provider "helm" {
  service_account = "${kubernetes_service_account.tiller_service.metadata.0.name}"
  namespace       = "${kubernetes_service_account.tiller_service.metadata.0.namespace}"

  kubernetes {
    config_path = "${local_file.kubeconfig.filename}"
  }
}

# setup nginx ingress - creats load balancer/ingress controller
resource "helm_release" "nginx_ingress" {
  depends_on = ["kubernetes_cluster_role_binding.tiller_sercice_roll", "kubernetes_deployment.webapp_deployment"]
  name       = "webappload-balancer"
  chart      = "stable/nginx-ingress"
}

data "kubernetes_service" "webapp_loadbalancer" {
  depends_on = ["helm_release.nginx_ingress"]

  metadata {
    name = "webappload-balancer-nginx-ingress-controller"
  }
}

output "loadbalancer_ip" {
  value = "${data.kubernetes_service.webapp_loadbalancer.load_balancer_ingress.0.ip}"
}

resource "digitalocean_domain" "default" {
  depends_on = ["helm_release.nginx_ingress"]
  name       = "${var.cluster-domain}"
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
    HOSTNAME       = "${var.cluster-domain}"
    SERVICENAME    = "${kubernetes_service.webapp_service.metadata.0.name}"
    SERVICEPORT    = "80"
  }
}

resource "null_resource" "ingress" {
  depends_on = ["helm_release.nginx_ingress", "digitalocean_domain.default", "digitalocean_record.mx1", "digitalocean_record.mx2"]

  triggers = {
    manifest_sha1 = "${sha1("${data.template_file.ingress.rendered}")}"
  }

  # NOTE - when you add a domain you - accessing from the IP address directly will return a 404
  provisioner "local-exec" {
    #command = "kubectl --kubeconfig ${path.module}/.kubeconfig apply -f -<<EOF\n${data.template_file.ingress.rendered}\nEOF"
    # Run "Set-Service ssh-agent -StartupType Manual" before using Powershell
    command = "@'\n${data.template_file.ingress.rendered}\n'@ | kubectl --kubeconfig ${path.module}/.kubeconfig apply -f -"

    # command = "kubectl --kubeconfig ${path.module}/.kubeconfig apply -f templates/ingress.yaml"
    interpreter = ["PowerShell", "-Command"]
  }
}

# Deploy Cert Manager
# see https://docs.cert-manager.io/en/latest/tutorials/acme/quick-start/
resource "null_resource" "certificates" {
  depends_on = ["null_resource.ingress"]

  # Install the cert-manager CRDs
  # IMPORTANT: you MUST install the cert-manager CRDs **before** installing the cert-manager Helm chart
  provisioner "local-exec" {
    command = "kubectl --kubeconfig ${path.module}/.kubeconfig apply -f https://raw.githubusercontent.com/jetstack/cert-manager/release-0.7/deploy/manifests/00-crds.yaml"
  }

  provisioner "local-exec" {
    when = "destroy"
    command = "echo 'destroy'" # TODO - reverse configuration on destroy (not exactly needed)
  }
}

# IMPORTANT: if the cert-manager namespace **already exists**, you MUST ensure it has an additional label on it in order for the deployment to succeed
resource "kubernetes_namespace" "cert-manager" {
  depends_on = ["null_resource.certificates"]

  metadata {
    name = "cert-manager"

    labels {
      "certmanager.k8s.io/disable-validation" = "true"
    }
  }
}

data "helm_repository" "jetstack" {
  name = "jetstack"
  url  = "https://charts.jetstack.io"
}

resource "helm_release" "cert-manager" {
  depends_on = ["null_resource.certificates", "kubernetes_namespace.cert-manager"]
  name       = "cert-manager"
  repository = "${data.helm_repository.jetstack.metadata.0.name}"
  chart      = "cert-manager"
  namespace  = "cert-manager"
}

# Configure Let's Encrypt Issuer
data "template_file" "issuer" {
  template = "${file("templates/issuer.yaml.tpl")}"

  vars {
    ISSUER_NAME             = "${var.issuer_name}"
    ISSUER_SERVER           = "${var.issuer_server}"
    CERTIFICATE_EMAIL       = "${var.certificate_email}"
    CERTIFICATE_SECRET_NAME = "${var.certificate_secret_name}"
  }
}

resource "null_resource" "issuer" {
  depends_on = ["helm_release.cert-manager"]

  triggers = {
    manifest_sha1 = "${sha1("${data.template_file.issuer.rendered}")}"
  }

  # NOTE - when you add a domain you - accessing from the IP address directly will return a 404
  provisioner "local-exec" {
    #command = "kubectl --kubeconfig ${path.module}/.kubeconfig apply -f -<<EOF\n${data.template_file.ingress.rendered}\nEOF"
    command = "@'\n${data.template_file.issuer.rendered}\n'@ | kubectl --kubeconfig ${path.module}/.kubeconfig create -f -"

    # command = "kubectl --kubeconfig ${path.module}/.kubeconfig apply -f templates/ingress.yaml"
    interpreter = ["PowerShell", "-Command"]
  }
}

# Deploy a TLS Ingress Resource
data "template_file" "ingress_issuer" {
  template = "${file("templates/ingress.yaml.tpl")}"

  vars {
    INGRESS_NAME   = "webapp-ingress"
    INCLUDE_ISSUER = true
    ISSUER_NAME    = "${var.issuer_name}"
    HOSTNAME       = "${var.cluster-domain}"
    SERVICENAME    = "${kubernetes_service.webapp_service.metadata.0.name}"
    SERVICEPORT    = "80"
  }
}

resource "null_resource" "ingress_issuer" {
  depends_on = ["null_resource.issuer"]

  triggers = {
    manifest_sha1 = "${sha1("${data.template_file.ingress_issuer.rendered}")}"
  }

  # NOTE - when you add a domain you - accessing from the IP address directly will return a 404
  provisioner "local-exec" {
    #command = "kubectl --kubeconfig ${path.module}/.kubeconfig apply -f -<<EOF\n${data.template_file.ingress.rendered}\nEOF"
    # Run "Set-Service ssh-agent -StartupType Manual" before using Powershell
    command = "@'\n${data.template_file.ingress_issuer.rendered}\n'@ | kubectl --kubeconfig ${path.module}/.kubeconfig apply -f -"

    # command = "kubectl --kubeconfig ${path.module}/.kubeconfig apply -f templates/ingress.yaml"
    interpreter = ["PowerShell", "-Command"]
  }
}
