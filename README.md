# Manage Your Education and Skills Funding View Your Payments Data Import Function
The Manage Your Education and Skills Funding View Your Payments Data Import Function app is used by the MYESF to allow the following:
- Calls the Business central API to fetch the payment details.
- Maps the retrieved data with the custom budget groups and save the payments to the VYP database.

## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is a .Net 8 time triggered Azure Function project utilizing an Azure Function App for deployment.

**Note:** The project is currently being updated to be containerised via Docker where the deployment method and target will change, this document will be updated when these changes have been finalised.

# Local Configuration Guide

For running the application locally, a `local.settings.json` file will need to be created in the `ViewYourPayments.DataImportFunction` project. Below, and included in the repo, there is `local.settings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Application Settings (`local.settings.json`)
```json
{
  "IsEncrypted": false,
  "Values": {
    "ConnectionStrings:ViewYourPaymentsDbContext": "",
    "APPINSIGHTS_INSTRUMENTATIONKEY": "",
    "APPLOGGINGAPPINSIGHTS_INSTRUMKEY": "",
    "AzureFunctionsJobHost__functionTimeout": "09:00:00",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "DataImportSetting:Company": "",
    "DataImportSetting:DataImportFromDate": "",
    "DataImportSetting:DataImportToDate": "",
    "DataImportSetting:DimensionCode": "",
    "DataImportSetting:DimensionValues": "",
    "DataImportSetting:NavApiBaseUrl": "",
    "DataImportSetting:NavAPICompaniesSettings:0:CompanyName": "",
    "DataImportSetting:NavAPICompaniesSettings:0:IncludeDimensionQuery": "",
    "DataImportSetting:NavAPICompaniesSettings:1:CompanyName": "",
    "DataImportSetting:NavAPICompaniesSettings:1:IncludeDimensionQuery": "",
    "DataImportSetting:NavApiSubscriptionKey": "",
    "DataImportSetting:NavApiVersionNumber": "",
    "DataImportSetting:RetryCounter": 3,
    "DataImportSetting:RetryGapDurationMilliseconds": 3000,
    "DataImportSetting:RunAsOneTimeJobWithConfigDates": false,
    "DataImportSetting:UseFinanceAPI": true,
    "Environment": "local",
    "FUNCTIONS_EXTENSION_VERSION": "~4",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "timerInterval": "0 15 12 * * *"
  }
}
```
### Setting Details

- **`ConnectionStrings:ViewYourPaymentsDbContext`**  
  The connection string for the View Your Payments database context.

- **`APPINSIGHTS_INSTRUMENTATIONKEY`**  
  The instrumentation key for Application Insights.

- **`APPLOGGINGAPPINSIGHTS_INSTRUMKEY`**  
  The instrumentation key for application logging with Application Insights.

- **`AzureFunctionsJobHost__functionTimeout`**  
  The maximum duration allowed for a single execution block to run before the system automatically terminates it.

- **`AzureWebJobsDashboard`**  
  An optional connection endpoint string formerly used to report runtime log events back to the legacy Azure classic dashboard platform.

- **`AzureWebJobsStorage`**  
  The foundational Azure Storage account connection details utilized internally by the infrastructure layer to manage essential system state files.

- **`DataImportSetting:Company`**  
  A comma-separated collection specifying which individual target entities should be parsed during execution loops.

- **`DataImportSetting:DataImportFromDate`**  
  The specific boundary floor timestamp marking the starting window for processing historical system transactions.

- **`DataImportSetting:DataImportToDate`**  
  The specific boundary ceiling timestamp marking the ending window for processing historical system transactions.

- **`DataImportSetting:DimensionCode`**  
  Dimension code for calling Business central API.

- **`DataImportSetting:DimensionValues`**  
  Dimension values in comma separated string for calling Business central API.

- **`DataImportSetting:NavApiBaseUrl`**  
  The root domain location path exposing the endpoint framework for your connected Microsoft Dynamics NAV software package.

- **`DataImportSetting:NavAPICompaniesSettings:0:CompanyName`**  
  First company name passed in Company parameter.

- **`DataImportSetting:NavAPICompaniesSettings:0:IncludeDimensionQuery`**  
  A flag value dictating whether metadata querying operations are enabled or bypassed for this specific company entry.

- **`DataImportSetting:NavAPICompaniesSettings:1:CompanyName`**  
  Second company name passed in Company parameter.

- **`DataImportSetting:NavAPICompaniesSettings:1:IncludeDimensionQuery`**  
  A flag value dictating whether metadata querying operations are enabled or bypassed for this specific company entry.

- **`DataImportSetting:NavApiSubscriptionKey`**  
  The security token key appended inside the HTTP header stack to authenticate access onto the API structure.

- **`DataImportSetting:NavApiVersionNumber`**  
  The direct version identifier string mapping your client endpoints against target system resources.

- **`DataImportSetting:RetryCounter`**  
  The number of times the function app will attempt to retry a failed operation before giving up.

- **`DataImportSetting:RetryGapDurationMilliseconds`**  
  The delay interval in milliseconds between successive retry attempts.

- **`DataImportSetting:RunAsOneTimeJobWithConfigDates`**  
  A state toggle determining whether to run the function app as a one time job with the provided FromDate and ToDate.

- **`DataImportSetting:UseFinanceAPI`**  
  A flag value dictating whether to utilize the Finance API for data import operations.

- **`Environment`**  
  A customized configuration value identifying the specific pipeline platform tier currently processing the data.

- **`FUNCTIONS_EXTENSION_VERSION`**  
  The specific runtime layer major release target under which this whole execution host runs inside the Cloud ecosystem.

- **`FUNCTIONS_WORKER_RUNTIME`**  
  The language model layer architecture strategy used to host processes outside the core execution block (e.g., `dotnet-isolated`).

- **`timerInterval`**  
  A standardized six-field CRON expression tracking when internal processes get automatically fired into action.

## Test execution

In order to test the application locally a valid `appsettings.json` file will need to be created in the `ViewYourPayments.DataImportFunction.Tests` project. `appsettings.example.json`, in `ViewYourPayments.DataImportFunction.Tests` can be used as a base and populated with appropriate values which can be found in Azure Portal. The local environment resources should be utilised.

## Test Application Settings (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "ViewYourPaymentsDbContext": ""
  }
}
```

### Setting Details

- **`ConnectionStrings:ViewYourPaymentsDbContext`**  
  The connection string for the ViewYourPayments database context. (Use `local` db connection string)

## Build and Test

To build and test locally, you can either use Visual Studio, Visual Studio Code or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

- If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
- If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.