variable "service_name" {}
variable "cluster_domain" {}
variable "issuer_name" {}
variable "issuer_server" {}
variable "certificate_email" {}
variable "certificate_secret_name" {}
variable "kube_config_path" {}
variable "wait_for_ingress" {
  description = "This is just a placeholder to ensure ingress is complete before this is run (should be replaced with module depends_on in newer Terraform)"
}

