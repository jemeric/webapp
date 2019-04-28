output "service_name" {
  value = "${kubernetes_service.webapp_service.metadata.0.name}"
}
