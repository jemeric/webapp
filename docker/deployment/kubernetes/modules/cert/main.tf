# Deploy Cert Manager
# see https://docs.cert-manager.io/en/latest/tutorials/acme/quick-start/
resource "null_resource" "certificates" {
  # depends_on = ["null_resource.ingress"] - should not be run until ingress is complete

  # Install the cert-manager CRDs
  # IMPORTANT: you MUST install the cert-manager CRDs **before** installing the cert-manager Helm chart
  provisioner "local-exec" {
    command = "kubectl --kubeconfig ${var.kube_config_path} apply -f https://raw.githubusercontent.com/jetstack/cert-manager/release-0.7/deploy/manifests/00-crds.yaml"
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
    command = "@'\n${data.template_file.issuer.rendered}\n'@ | kubectl --kubeconfig ${var.kube_config_path} create -f -"

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
    HOSTNAME       = "${var.cluster_domain}"
    SERVICENAME    = "${var.service_name}"
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
    command = "@'\n${data.template_file.ingress_issuer.rendered}\n'@ | kubectl --kubeconfig ${var.kube_config_path} apply -f -"

    # command = "kubectl --kubeconfig ${path.module}/.kubeconfig apply -f templates/ingress.yaml"
    interpreter = ["PowerShell", "-Command"]
  }
}
