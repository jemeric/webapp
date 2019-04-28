output "service_name" {
  value = "${kubernetes_service.webapp_service.metadata.0.name}"
}

output "service_port" {
  value = "${kubernetes_service.webapp_service.spec.port.0.port}"
}
