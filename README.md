---
name: Ticket Processing with Azure AI
description: Process tickets automatically with Azure AI LLMs and Speech Service.
languages:
- DotNet
- bicep
- azdeveloper
- Prompty
products:
- azure-openai
- azure-cognitive-search
- azure-app-service
- azure
page_type: sample
urlFragment:  summarization-openai-csharp-prompty
---

# Automated Ticket Processing using Azure AI

---

# Table of Contents

- [What is this sample?](#what-is-this-sample)
- [Features](#features)
- [Architecture Diagram](#architecture-diagram)
- [Security](#security)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Quickstart](#quickstart)
  - [Local Development](#local-development)
- [Costs](#costs)  
- [Security Guidelines](#security-guidelines)
- [Resources](#resources)

[![Open in GitHub Codespaces](https://img.shields.io/static/v1?style=for-the-badge&label=GitHub+Codespaces&message=Open&color=brightgreen&logo=github)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=599293758&machine=standardLinux32gb&devcontainer_path=.devcontainer%2Fdevcontainer.json&location=WestUs2)
[![Open in Dev Containers](https://img.shields.io/static/v1?style=for-the-badge&label=Dev%20Containers&message=Open&color=blue&logo=visualstudiocode)](https://vscode.dev/redirect?url=vscode://ms-vscode-remote.remote-containers/cloneInVolume?url=https://github.com/azure-samples/azure-search-openai-demo)

# What is this sample

In this sample we recieve issues reported by field and shop floor workers at a company called Contoso Manufacturing, a manufacturing company that makes car batteries. The issues are shared by the workers either live through microphone input or pre-recorded as audio files. We translate the input from speech to text and then use an LLM and Prompty or Promptflow to summarize the issue and return the results in a format we specify.

This sample uses the **[Azure AI Speech Service](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/)** and **[Python SDk](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/quickstarts/setup-platform?pivots=programming-language-python&tabs=macos%2Cubuntu%2Cdotnetcli%2Cdotnet%2Cjre%2Cmaven%2Cnodejs%2Cmac%2Cvscode)** to translate the users speech into text. It leverages **Azure OpenAI** to summarize the text and **Prompty/Prompt Flow** to manage and insert the prompt into our code, and to evaluate prompt/LLM performance.

By the end of deploying this template you should be able to:

 1. Describe what Azure AI Speech Service Python SDK provides.
 2. Explain prompt creation with Prompty/Prompt Flow. 
 3. Build, run, evaluate, and deploy, the summarization app to Azure.

#
# Features

The project comes with:

- Sample **model configurations**, **evaluation prompts**, and **Prompty** assets (to simplify prompt creation & iteration) for a RAG-based copilot application
- Sample **product and customer data** for retail application scenario
- Sample **application code** for copilot chat and evaluation functions
- Sample **azd-template configuration** for managing application on Azure

The sample is also a signature application for demonstrating the new capabilities of the Azure AI platform. Expect regular updates to showcase cutting-edge features and best practices for generative AI development. 

## Architecture Diagram
![Architecture Diagram](/images/summarization.png)

# Getting Started

## Prerequisites

### Azure Account 

**IMPORTANT:** In order to deploy and run this example, you'll need:

* **Azure account**. If you're new to Azure, [get an Azure account for free](https://azure.microsoft.com/free/cognitive-search/) and you'll get some free Azure credits to get started. See [guide to deploying with the free trial](docs/deploy_lowcost.md).
* **Azure subscription with access enabled for the Azure OpenAI service**. You can request access with [this form](https://aka.ms/oaiapply). If your access request to Azure OpenAI service doesn't match the [acceptance criteria](https://learn.microsoft.com/legal/cognitive-services/openai/limited-access?context=%2Fazure%2Fcognitive-services%2Fopenai%2Fcontext%2Fcontext), you can use [OpenAI public API](https://platform.openai.com/docs/api-reference/introduction) instead. Learn [how to switch to an OpenAI instance](docs/deploy_existing.md#openaicom-openai).
    - Ability to deploy these models - `gpt-35-turbo`, `gpt-4`, `text-embeddings-ada-002`
    - We recommend using Sweden Central or East US 2
* **Azure account permissions**:
  * Your Azure account must have `Microsoft.Authorization/roleAssignments/write` permissions, such as [Role Based Access Control Administrator](https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#role-based-access-control-administrator-preview), [User Access Administrator](https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#user-access-administrator), or [Owner](https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#owner). If you don't have subscription-level permissions, you must be granted [RBAC](https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#role-based-access-control-administrator-preview) for an existing resource group and [deploy to that existing group](docs/deploy_existing.md#resource-group).
  * Your Azure account also needs `Microsoft.Resources/deployments/write` permissions on the subscription level.
- **Ability to provision Azure AI Search (Paid)** - Required for Semantic Ranker
    - We recommend using East US 2    
- **Ability to provision Azure Monitor (Free tier)**
- **Ability to deploy to Azure Container Apps (Free tier)**

### AZD
- **Install [azd](https://aka.ms/install-azd)**
    - Windows: `winget install microsoft.azd`
    - Linux: `curl -fsSL https://aka.ms/install-azd.sh | bash`
    - MacOS: `brew tap azure/azd && brew install azd`




# Architecture Diagram
Include a diagram describing the application (DevDiv is working with Designers on this part)

# Security

(Document security aspects and best practices per template configuration)

* ex. keyless auth

We can show how to set up keyless auth for the speech sdk with azd. More detailed info here: https://learn.microsoft.com/en-us/azure/ai-services/speech-service/role-based-access-control 
```
#grant permissions using azd (this assigns a Cognitive Services User role)
 az role assignment create \
        --role "f2dc8367-1007-4938-bd23-fe263f013447" \
        --assignee-object-id "$PRINCIPAL_ID" \
        --scope /subscriptions/"$SUBSCRIPTION_ID"/resourceGroups/"$RESOURCE_GROUP" \
        --assignee-principal-type User

```

# Getting Started

## Prerequisites

- Install [azd](https://aka.ms/install-azd)
    - Windows: `winget install microsoft.azd`
    - Linux: `curl -fsSL https://aka.ms/install-azd.sh | bash`
    - MacOS: `brew tap azure/azd && brew install azd`
- OS
- Library version
- This model uses [MODEL 1] and [MODEL 2] which may not be available in all Azure regions. Check for [up-to-date region availability](https://learn.microsoft.com/azure/ai-services/openai/concepts/models#standard-deployment-model-availability) and select a region during deployment accordingly
    - We recommend using East US

- ...

## Quickstart
(Add steps to get up and running quickly)
 
1. Clone the repository and intialize the project: `azd init [name-of-repo]`
2. ...
3. Provision and deploy the project to Azure: `azd up`
4. Set up CI/CD with `azd pipeline config`
5. (Add steps to start up the sample app)

## Local Development
Describe how to run and develop the app locally


## Costs
You can estimate the cost of this project's architecture with [Azure's pricing calculator](https://azure.microsoft.com/pricing/calculator/)

- Azure OpenAI - Standard tier, GPT-4, GPT-35-turbo and Ada models.  [See Pricing](https://azure.microsoft.com/pricing/details/cognitive-services/openai-service/)
- Azure Monitor - Serverless, Free Tier [See Pricing](https://azure.microsoft.com/en-us/pricing/details/monitor/)
- Azure Container Apps - Severless, Free Tier [See Pricing](https://azure.microsoft.com/en-us/pricing/details/container-apps/)


# Security Guidelines

This template uses [Managed Identity](https://learn.microsoft.com/en-us/entra/identity/managed-identities-azure-resources/overview) or Key Vault to eliminate the need for developers to manage credentials. Applications can use managed identities to obtain Microsoft Entra tokens without having to manage any credentials.

Additionally, we have added a [GitHub Action tool](https://github.com/microsoft/security-devops-action) that scans the infrastructure-as-code files and generates a report containing any detected issues. 

To ensure best security practices in your repo, we recommend anyone creating solutions based on our templates ensure that the [Github secret scanning](https://docs.github.com/en/code-security/secret-scanning/about-secret-scanning) setting is enabled in your repos.

# Resources

- [Take a look on more .NET AI Samples.](https://github.com/dotnet/ai-samples/)
- [Learn more .NET AI with Microsoft Learn](https://learn.microsoft.com/pt-pt/dotnet/azure/)
- [Learn Azure, deploying in GitHub!](https://github.com/Azure-Samples)

## Troubleshooting

Have questions or issues to report? Please [open a new issue](https://github.com/Azure-Samples/summarization-openai-csharp-prompty/issues) after first verifying that the same question or issue has not already been reported. In the latter case, please add any additional comments you may have, to the existing issue.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
