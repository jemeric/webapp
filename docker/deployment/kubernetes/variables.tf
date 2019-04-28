variable "cluster_name" {
  description = "The name of the Kubernetes cluster (i.e. the name that will show up in the DigitalOcean console)"
}

variable "cluster_region" {
  description = "The DigitalOcean cluster region (e.g. nyc1)"
}

variable "cluster_version" {
  description = "The DigitalOcean Kubernetes cluster version (e.g. 1.13.1-do.2)"
}

variable "cluster_node_pool_name" {
  description = "The name given to the Kubernetes node pool in DigitalOcean"
}

variable "cluster_node_pool_size" {
  description = "The size of node to use in the node pool (e.g. s-1vcpu-2gb)"
}

variable "cluster_node_pool_node_count" {
  description = "The number of nodes that will form the node pool (e.g. 3)"
}

variable "cluster_domain" {
  description = "The domain name that will be assigned to this app (e.g. yourdomain.com)"
}

variable "issuer_name" {
  description = "The name of ther certificate issuer (e.g. letsencrypt-staging or letsencrypt-prod)"
}

variable "issuer_server" {
  description = "The issuer server location (e.g. https://acme-staging-v02.api.letsencrypt.org/directory or https://acme-v02.api.letsencrypt.org/directory)"
}

variable "certificate_email" {
  description = "The email address that will be used to register your domain with the issuer (e.g. letsencrypt@yourdomain.com - see MX records config to create this automatically)"
}

variable "certificate_secret_name" {
  description = "The name given to the certificate secret (e.g. letsencrypt-staging or letsencrypt-prod)"
}

variable "webapp_image" {
  description = "The web application docker image that will be used to create containers in the cluster's pods (e.g. docker.io/username/image:tag)"
}

variable "webapp_container_port" {
  description = "The port that will be exposed by the webapp container (e.g. 5001)"
}

variable "pod_replicas" {
  description = "The number of Kubernetes pod replicas that will be created in the deployment (e.g. 3)"
}
