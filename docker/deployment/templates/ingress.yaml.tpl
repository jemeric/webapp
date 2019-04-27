apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: ${INGRESS_NAME}
  annotations:
    kubernetes.io/ingress.class: "nginx"
%{ if INCLUDE_ISSUER }
    certmanager.k8s.io/issuer: ${ISSUER_NAME}
    certmanager.k8s.io/acme-challenge-type: http01
%{ endif }
spec:
  tls:
  - hosts:
    - ${HOSTNAME}
    secretName: quickstart-example-tls
  rules:
  - host: ${HOSTNAME}
    http:
      paths:
      - path: /
        backend:
          serviceName: ${SERVICENAME}
          servicePort: ${SERVICEPORT}