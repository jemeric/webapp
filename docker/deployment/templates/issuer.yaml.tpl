   apiVersion: certmanager.k8s.io/v1alpha1
   kind: Issuer
   metadata:
     name: ${ISSUER_NAME}
   spec:
     acme:
       # The ACME server URL
       server: ${ISSUER_SERVER}
       # Email address used for ACME registration
       email: ${CERTIFICATE_EMAIL}
       # Name of a secret used to store the ACME account private key
       privateKeySecretRef:
         name: ${CERTIFICATE_SECRET_NAME}
       # Enable the HTTP-01 challenge provider
       http01: {}