# gives so-called virtual IP (cluster IP) for the pods that have a certain label
# and those pods are basically app containers you deployed with deployment your 
# app's container with some specifications (gives pods a stable IP)
resource "kubernetes_service" "webapp_service" {
  metadata {
    name = "webapp-service"
  }

  spec {
    selector {
      app = "${kubernetes_deployment.webapp_deployment.metadata.0.labels.app}"
    }

    port {
      protocol    = "TCP"
      port        = "80"
      target_port = 5001  # targets TCP port 5001 on any pod with "app={app_label}"
    }
  }
}

# create docker secret so we can download from private repository
resource "kubernetes_secret" "webapp_secret" {
  metadata {
    name = "docker-cfg"
  }

  data {
    # credStore's currently not supported - may need to rebuild this file (https://stackoverflow.com/a/47003541)
    ".dockerconfigjson" = "${file(pathexpand("~/.docker/config.json"))}"
  }
  
  type = "kubernetes.io/dockerconfigjson"
}

# manages pods in nodes
resource "kubernetes_deployment" "webapp_deployment" {
  metadata {
    name = "webapp-deployment"

    labels {
      app = "webapp"
    }
  }

  spec {
    replicas = 3 # create 3 replicated pods

    selector {
      # defines how the deployment finds which pods to manage (select label that is defined in Pod template, app: webapp)
      match_labels {
        app = "webapp"
      }
    }

    template {
      metadata {
        labels {
          app = "webapp"
        }
      }

      spec {
        container {
          # indicates containers Pods run in
          #image = "docker.io/jericmason/webapp:prod"
          image = "docker.io/jericmason/production:private"
          name  = "webapp"

          port {
            container_port = 5001 # open port so container can send/accept traffic
          }
        }
        image_pull_secrets {
          name = "${kubernetes_secret.webapp_secret.metadata.0.name}"
        }
      }
    }
  }
}