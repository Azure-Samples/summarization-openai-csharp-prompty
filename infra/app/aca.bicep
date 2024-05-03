param name string
param location string = resourceGroup().location
param tags object = {}

param identityName string
param identityId string
param containerAppsEnvironmentName string
param containerRegistryName string
param serviceName string = 'aca'
param openAiDeploymentName string
param openAiEndpoint string
param openAiApiVersion string
param openAiType string
param appinsights_Connectionstring string


module app '../core/host/container-app-upsert.bicep' = {
  name: '${serviceName}-container-app-module'
  params: {
    name: name
    location: location
    tags: union(tags, { 'azd-service-name': serviceName })
    identityName: identityName
    identityType: 'UserAssigned'
    containerAppsEnvironmentName: containerAppsEnvironmentName
    containerRegistryName: containerRegistryName
    env: [
      {
        name: 'AZURE_CLIENT_ID'
        value: identityId
      }
      {
        name: 'OPENAI__TYPE'
        value: openAiType
      }
      {
        name: 'OPENAI__API_VERSION'
        value: openAiApiVersion
      }
      {
        name: 'OPENAI__ENDPOINT'
        value: openAiEndpoint
      }
      {
        name: 'OPENAI__DEPLOYMENT'
        value: openAiDeploymentName
      }
      {
        name: 'APPLICATIONINSIGHTS__CONNECTIONSTRING'
        value: appinsights_Connectionstring
      }

    ]
    targetPort: 50505
  }
}

output SERVICE_ACA_NAME string = app.outputs.name
output SERVICE_ACA_URI string = app.outputs.uri
output SERVICE_ACA_IMAGE_NAME string = app.outputs.imageName
