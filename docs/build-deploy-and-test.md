# Build Deploy and Test the implementation

This document contains the detailed steps to build, deploy and test the implementation.

## Prerequisites

1. [Microsoft Power Platform Environment](https://learn.microsoft.com/en-us/power-platform/admin/environments-overview) with:
    1. Premium license for premium connectors
    1. Admin access to the environment
1. [Azure Subscription](https://azure.microsoft.com/en-in/free/)
    1. In the same Tenant as the Power Platform Environment
    1. Admin access to the subscription
1. Development machine (Windows OS preferred) with following installed:
    1. [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
    1. [.NET Core 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Provision Azure Resources

1. Open Azure Portal and create a new resource group by following the steps [here](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/manage-resource-groups-portal#create-resource-groups).
1. Deploy the resources using [ARM Template](../IaC/arm_template.json) in [Custom Deployment](https://portal.azure.com/#create/Microsoft.Template) - by following the steps [here](https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/deploy-portal#deploy-resources-from-custom-template)
1. Once deployed the following resources will be created:
    1. Azure App Service Plan
    1. Azure App Service
    1. Azure Log Analytics Workspace
    1. Azure Application Insights

## Authentication setup

1. Create a new Azure AD App Registration by following the steps [here](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app) in the same Tenant as the Power Platform Environment.
1. Assign the following permissions by following the steps [here](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-access-web-apis#add-permissions-to-access-your-web-api) to the App Registration:
    1. `Dynamics CRM` - `user_impersonation`
    1. `Microsoft Graph` - `User.Read`
1. Create a new client secret by following the steps [here](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app#add-credentials) for the App Registration.
1. In [Power Platform Admin Center](https://admin.powerplatform.microsoft.com/) add the Azure AD App Registration as a app user with system administrator permission.
1. Make a note of the following:
    1. Client ID
    1. Client Secret
    1. Tenant ID
    1. Power Platform Environment URL
    1. Azure Application Insights Connection String

## Configure the Azure App Service

1. Set the following Application settings in the Azure App Service under `Configuration` -> `Application settings`:
    1. `WEBSITE_PORT` - `443`
    1. `DataverseServicePrincipal__BaseUrl` - Power Platform Environment URL prefixed with `https://`
    1. `DataverseServicePrincipal__ClientId` - Client ID
    1. `DataverseServicePrincipal__ClientSecret` - Client Secret
    1. `DataverseServicePrincipal__TenantId` - Tenant ID
    1. `DataverseServicePrincipal__AuthDelegationUrl` - Power Platform Environment URL prefixed with `https://` and suffixed with `/.default`
    1. `ApplicationInsights__ConnectionString` - Azure Application Insights Connection String
1. Set the following General settings in the Azure App Service under `Configuration` -> `General settings`:
    1. `Stack` - `.NET`
    1. `Major version` - `.NET 7 (STS)`
    1. `Minor version` - `.NET 7 (STS)`
    1. `HTTPS Only` - `On`

## Build and Deploy the Azure App Service

### Build

1. Clone the repository.
1. Update the git submodule for the Dataverse REST API client by executing `git submodule update --init --recursive`
1. Open the solution file [DataverseBulkDataIntegration.sln](../DataverseBulkDataIntegration/DataverseBulkDataIntegration.sln) in Visual Studio 2022.
1. Build the solution by clicking on `Build` -> `Build Solution` in Visual Studio.

### Deploy

1. Create a Publish profile for the solution to the Azure App Service by clicking on the following menu in Visual Studio:
    1. `Publish`
    1. `Create Profile`
    1. Select Target as `Azure`
    1. Select Specific Target as `Azure App Service (Linux)`
    1. Select the Subscription and App Service created in the previous step
1. Publish the solution by clicking on the following menu in Visual Studio:
    1. `Publish`
    1. Select the Publish profile created in the previous step and click `Publish`

### Unit Test and Run locally (optional step for development)

1. In Visual Studio, open the solution explorer and right click on the project `ExcelImportService` and select `Manage User Secrets` and update the following JSON

    ```json
    {
        "DataverseServicePrincipal": {
            "BaseUrl": "Power Platform Environment URL prefixed with `https://`",
            "ClientId": "<Client ID>",
            "ClientSecret": "<Client Secret>",
            "TenantId": "<Tenant ID>",
            "AuthDelegationUrl": "Power Platform Environment URL prefixed with `https://` and suffixed with `/.default`"
        },
        "ApplicationInsights": {
            "ConnectionString": "<Azure Application Insights Connection String>"
        }
    }
    ```

1. In Visual Studio, open the solution explorer and right click on the project `ExcelImportService` and select `Set as Startup Project`
1. In Visual Studio, press `F5` to run the project locally.
1. To run unit test cases, right click on the project `ExcelImportService` and select `Run Unit Tests`

## Deploy the Power Platform Solution

1. Import the solution file [ContosoBudgetManagement_1_0_0_1.zip](https://github.com/Azure-Samples/PowerPlatform-AzureAppService-Sample/releases/download/powerplatform-solution/ContosoBudgetManagement_1_0_0_1.zip) into your Power Platform environment by following the steps [here](https://learn.microsoft.com/en-us/power-apps/maker/data-platform/import-update-export-solutions)
1. Import the sample master data for following tables by following the steps [here](https://learn.microsoft.com/en-us/power-apps/maker/data-platform/data-platform-import-export#import-the-file)
    1. `Corporation` -  [contoso_corporations.csv](../PowerPlatform/data/contoso_corporations.csv)
    1. `Department` -  [contoso_departments.csv](../PowerPlatform/data/contoso_departments.csv)
    1. `Budget Category` -  [contoso_budgetcategories.csv](../PowerPlatform/data/contoso_budgetcategories.csv)
1. Provide the value for the environment variable `contoso_AZURE_WEB_APP_URL` in the Power Platform environment by following the steps [here](https://learn.microsoft.com/en-us/power-apps/maker/data-platform/environmentvariables#enter-new-values-while-importing-solutions).
    1. The value should be the URL of the Azure App Service created in the previous step prefixed with `https://`.
1. Edit Cloud flow `TriggerAzureWebAppToCreateBudget` by following the steps [here](https://learn.microsoft.com/en-us/power-automate/edit-solution-aware-flow)
    1. Follow the instruction to update the `Connection References` for Dataverse.
    1. `Turn on` the flow.

## Test the Implementation

1. Open the Power Platform Solution `Contoso Budget Management` in the Power Platform environment by following steps [here](https://learn.microsoft.com/en-us/power-apps/maker/data-platform/solutions-overview)
1. Play the App `Contoso Budget Management`
1. Once the app is opened, change the area to `Settings` and validate is the master data is imported correctly in all tables - `Corporation`, `Department` and `Budget Category`.
1. Change the area to `Budget`, click on the `Import Budget` button in the `Budget Management` section.
1. Create a new record and add `Attachment` from the following files:
    1. Success scenario - [ImportBudgetSuccess.xlsx](../PowerPlatform/sample/ImportBudgetSuccess.xlsx)
    1. Failure scenario - [ImportBudgetFailure.xlsx](../PowerPlatform/sample/ImportBudgetFailure.xlsx)
    1. Template that can be used to create new file - [ImportBudgetTemplate.xlsx](../PowerPlatform/sample/ImportBudgetTemplate.xlsx)
1. Once the file is added, the status will be changed to `In Progress` and and followed by `Success` or `Error` based on the status of the import.
1. The uploaded budget data can be viewed by clicking to `View Budget` button in the `Budget Management` section.

## References

* [Learn to code in Visual Studio](https://visualstudio.microsoft.com/vs/getting-started/)
* [Learn Microsoft Power Platform](https://learn.microsoft.com/en-us/training/powerplatform/)
* [Learn .NET](https://dotnet.microsoft.com/en-us/learn)
