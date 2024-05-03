#!/bin/bash

echo  "Building summarizationapi:latest..."
az acr build --subscription ${AZURE_SUBSCRIPTION_ID} --registry ${AZURE_CONTAINER_REGISTRY_NAME} --image summarizationapi:latest ./src/SummarizationAPI/
image_name="${AZURE_CONTAINER_REGISTRY_NAME}.azurecr.io/summarizationapi:latest"
az containerapp update --subscription ${AZURE_SUBSCRIPTION_ID} --name ${SERVICE_ACA_NAME} --resource-group ${RESOURCE_GROUP_NAME} --image ${image_name}
az containerapp ingress update --name ${SERVICE_ACA_NAME} --resource-group ${RESOURCE_GROUP_NAME} --target-port 8080
