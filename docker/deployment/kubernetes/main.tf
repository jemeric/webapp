# provider blocked used named provider (i.e. digitalocean, aws) - provider is responsible for creating/managing resources
# use environment variable DIGITALOCEAN_TOKEN to provide access token (https://www.terraform.io/docs/providers/do/index.html#token)
# modules inherit un-aliased default provider by default
provider "digitalocean" {}

# Create a Kubernetes Cluster (creates nodes)
# In Terraform second resource string declare name used to refer to this resource from elsewhere in Terraform module
# it has no significance outside the scope of the module
resource "digitalocean_kubernetes_cluster" "cluster" {
  name    = "${var.cluster_name}"
  region  = "${var.cluster_region}"
  version = "${var.cluster_version}"
  tags    = ["${var.cluster_name}"]

  node_pool {
    name       = "${var.cluster_node_pool_name}"
    size       = "${var.cluster_node_pool_size}"
    node_count = "${var.cluster_node_pool_node_count}"
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

module "application" {
  source = "./modules/app"
}

provider "helm" {
  service_account = "${module.network.tiller_service_name}"
  namespace       = "${module.network.tiller_service_namespace}"

  kubernetes {
    config_path = "${local_file.kubeconfig.filename}"
  }
}

module "network" {
  source = "./modules/ingress"
  kube_config_path = "${local_file.kubeconfig.filename}"
  service_name = "${module.application.service_name}"
  cluster_domain = "${var.cluster_domain}"
  issuer_name = "${var.issuer_name}"
  issuer_server = "${var.issuer_server}"
  certificate_email = "${var.certificate_email}"
  certificate_secret_name = "${var.certificate_secret_name}"
}

module "certificates" {
  source = "./modules/cert"
  kube_config_path = "${local_file.kubeconfig.filename}"
  service_name = "${module.application.service_name}"
  cluster_domain = "${var.cluster_domain}"
  issuer_name = "${var.issuer_name}"
  issuer_server = "${var.issuer_server}"
  certificate_email = "${var.certificate_email}"
  certificate_secret_name = "${var.certificate_secret_name}"
  wait_for_ingress = "${module.network.wait_for_ingress}" # TODO replace with module depends_on when upgraded to version tf 0.12
}
