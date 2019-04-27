variable "cluster-name" {
    description = "The name of the Kubernetes cluster"
}
variable "cluster-region" {}
variable "cluster-version" {}
variable "cluster-node-pool-name" {}
variable "cluster-node-pool-size" {}
variable "cluster-node-pool-node-count" {}
variable "cluster-domain" {}
variable "issuer_name" {}
variable "issuer_server" {}
variable "certificate_email" {}
variable "certificate_secret_name" {}
