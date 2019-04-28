output "tiller_service_name" {
  value = "${kubernetes_service_account.tiller_service.metadata.0.name}"
}

output "tiller_service_namespace" {
  value = "${kubernetes_service_account.tiller_service.metadata.0.namespace}"
}

output "wait_for_ingress" {
  value = "${null_resource.noop.id}"
}
