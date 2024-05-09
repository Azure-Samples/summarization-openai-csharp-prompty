---
name: Ticket Processing with Azure AI
description: Process tickets automatically with Azure AI LLMs and Speech Service.
languages:
- TBD
products:
- azure-openai
- azure-ai-speech-service
- azure
page_type: sample
urlFragment: TBD
---

# Ticket Processing using Azure AI

In this sample we recieve issues reported by field and shop floor workers at a company called Contoso Manufacturing, a manufacturing company that makes car batteries. The issues are shared by the workers either live through microphone input or pre-recorded as audio files. We translate the input from speech to text and then use an LLM and Prompty/Promptflow to summarize the issue and return the results in a format we specify.

Samples in JavaScript, .NET, and Java. Learn more at https://aka.ms/azai.

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

# Ticket Processing using Azure AI 

This sample uses the **[Azure AI Speech Service](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/)** and **[Semantic Kernel SDK](https://learn.microsoft.com/en-us/semantic-kernel/overview/?tabs=Csharp)** to translate the users speech into text. It leverages **Azure OpenAI** to summarize the text and **Prompty/Semantic Kernel** to manage and insert the prompt into our code, and to evaluate prompt/LLM performance.

By the end of deploying this template you should be able to:

 1. Describe what Azure AI Speech Service Semantic Kernel SDK provides.
 2. Explain prompt creation with Prompty/Semantic Kernel. 
 3. Build, run, evaluate, and deploy, the summarization app to Azure.

# Features

This project template provides the following features:

* A `waiting for speech to text PR` file that converts microphone input or pre-recorded audio to text.
* Pre-recorded audio files in the `summarization-openai-charp-prompty/data/audio-data` folder to use for testing the app.
* A `summarize.prompty` file where the prompt is constructed and edited.
* A `SummarizationAPI.csproj` file with all the packages needed to run this example.
* Built-in evaluations to test your prompt template built using prompty against a variety of test datasets with telemetry pushed to Azure AI Studio
* You will be able to use this app with Azure AI Studio


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

### Azure Account 

**IMPORTANT:** In order to deploy and run this example, you'll need:

* **Azure account**. If you're new to Azure, [get an Azure account for free](https://azure.microsoft.com/free/cognitive-search/) and you'll get some free Azure credits to get started. See [guide to deploying with the free trial](docs/deploy_lowcost.md).
* **Azure subscription with access enabled for the Azure OpenAI service**. You can request access with [this form](https://aka.ms/oaiapply). If your access request to Azure OpenAI service doesn't match the [acceptance criteria](https://learn.microsoft.com/legal/cognitive-services/openai/limited-access?context=%2Fazure%2Fcognitive-services%2Fopenai%2Fcontext%2Fcontext), you can use [OpenAI public API](https://platform.openai.com/docs/api-reference/introduction) instead. Learn [how to switch to an OpenAI instance](docs/deploy_existing.md#openaicom-openai).
* **Azure account permissions**:
  * Your Azure account must have `Microsoft.Authorization/roleAssignments/write` permissions, such as [Role Based Access Control Administrator](https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#role-based-access-control-administrator-preview), [User Access Administrator](https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#user-access-administrator), or [Owner](https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#owner). If you don't have subscription-level permissions, you must be granted [RBAC](https://learn.microsoft.com/azure/role-based-access-control/built-in-roles#role-based-access-control-administrator-preview) for an existing resource group and [deploy to that existing group](docs/deploy_existing.md#resource-group).
  * Your Azure account also needs `Microsoft.Resources/deployments/write` permissions on the subscription level.


Once you have an Azure account you have two options for setting up this project. The easiest way to get started is GitHub Codespaces, since it will setup all the tools for you, but you can also set it up [locally]() if desired.

### Github Codespaces 

You can run this repo virtually by using GitHub Codespaces, which will open a web-based VS Code in your browser:
[Github Codespaces](https://codespaces.new/Azure-Samples/summarization-openai-csharp-prompty)

### Local Environment 

- Install [azd](https://aka.ms/install-azd)
    - Windows: `winget install microsoft.azd`
    - Linux: `curl -fsSL https://aka.ms/install-azd.sh | bash`
    - MacOS: `brew tap azure/azd && brew install azd`
- ToDO C#/.NET version requirements
- [Git](https://git-scm.com/downloads)
- [Powershell 7+ (pwsh)](https://github.com/powershell/powershell) - For Windows users only.
    Important: Ensure you can run pwsh.exe from a PowerShell terminal. If this fails, you likely need to upgrade PowerShell.
- This sample uses `gpt-3.5-turbo` and [OpenAI text to speech models](https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/models#text-to-speech-preview) which may not be available in all Azure regions. Check for [up-to-date region availability](https://learn.microsoft.com/azure/ai-services/openai/concepts/models#standard-deployment-model-availability) and select a region during deployment accordingly
    - We recommend using `swedencentral`for Azure OpenAI and `eastus` for the speech to text services 

## Quickstart
(Add steps to get up and running quickly)
 
1. Clone the repository and intialize the project: 
```
azd init summarization-openai-csharp-prompty
```
Note that this command will initialize a git repository, so you do not need to clone this repository.

2. Login to your Azure account:
```
azd auth login
```

3. Create a new azd environment:
```
azd env new
```
Enter a name that will be used for the resource group. This will create a new folder in the .azure folder, and set it as the active environment for any calls to azd going forward.

4. Provision and deploy the project to Azure: `azd up`
6. Set up CI/CD with `azd pipeline config`
7. ToDo STEPS TO RUN APP WITH .NET/C#

## Local Development
Describe how to run and develop the app locally

# Costs
You can estimate the cost of this project's architecture with [Azure's pricing calculator](https://azure.microsoft.com/pricing/calculator/)
 
- Azure OpenAI: Standard tier, GPT and Ada models. Pricing per 1K tokens used, and at least 1K tokens are used per question. [Pricing](https://azure.microsoft.com/pricing/details/cognitive-services/openai-service/)
- Azure AI Speech: Pay as you go, Standard,	$1 per hour [Pricing](https://azure.microsoft.com/en-gb/pricing/details/cognitive-services/speech-services/)

# Security Guidelines

We recommend using keyless authentication for this project. Read more about why you should use managed identities on our [blog](https://techcommunity.microsoft.com/t5/microsoft-developer-community/using-keyless-authentication-with-azure-openai/ba-p/4111521). 

# Resources

(Any additional resources or related projects)
 
- Link to supporting information
- Link to similar sample
- [Develop Python apps that use Azure AI services](https://learn.microsoft.com/azure/developer/python/azure-ai-for-python-developers)
- ...
