variable "service_name" {
  description = "The name of the Kubernetes service deployed to the cluster"
}

variable "service_port" {
  description = "The TCP port exposed by the service"
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

variable "kube_config_path" {
  description = "The path to the Kubernetes config file for this cluster (only works for Kubernetes secrets credentials are in the file, i.e. its not using credStore)"
}

variable "wait_for_ingress" {
  description = "This is just a placeholder to ensure ingress is complete before this is run (should be replaced with module depends_on in newer Terraform)"
}
