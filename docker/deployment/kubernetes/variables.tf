variable "cluster_name" {
    description = "The name of the Kubernetes cluster"
}
variable "cluster_region" {}
variable "cluster_version" {}
variable "cluster_node_pool_name" {}
variable "cluster_node_pool_size" {}
variable "cluster_node_pool_node_count" {}
variable "cluster_domain" {}
variable "issuer_name" {}
variable "issuer_server" {}
variable "certificate_email" {}
variable "certificate_secret_name" {}
