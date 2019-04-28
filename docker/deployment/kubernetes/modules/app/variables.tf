variable "webapp_image" {
  description = "The web application docker image that will be used to create containers in the cluster's pods (e.g. docker.io/username/image:tag)"
}

variable "webapp_container_port" {
  description = "The port that will be exposed by the webapp container (e.g. 5001)"
}

variable "pod_replicas" {
  description = "The number of Kubernetes pod replicas that will be created in the deployment (e.g. 3)"
}
