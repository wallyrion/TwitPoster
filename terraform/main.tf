
 #Comment out or remove this section
 terraform {
   backend "azurerm" {
     resource_group_name  = "tfstate-rg"
     storage_account_name = "tfstate12345"
     container_name       = "tfstate"
     key                  = "terraform.tfstate"
   }
 }

#terraform {
#  backend "local" {
#    path = "terraform.tfstate"
#  }
#}


provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "twitposter-${var.environment}-rg"
  location = "East US"
}

resource "azurerm_service_plan" "function_app_plan" {
  name                = "twitposter-${var.environment}-function-app-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "Y1"
}

resource "azurerm_storage_account" "storage" {
  name                     = "twitpostersa${var.environment}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_servicebus_namespace" "sbnamespace" {
  name                     = "twitposter-sb${var.environment}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  sku                      = "Standard"
}

resource "azurerm_log_analytics_workspace" "analyticsWorkspace" {
  name                = "analyticsWorkspace-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "appinsights" {
  name                = "twitposter-ai-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.analyticsWorkspace.id
}

resource "azurerm_application_insights" "appinsights_emailsender" {
  name                = "twitposter-ai-emailsender-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.analyticsWorkspace.id
}

resource "azurerm_mssql_server" "sqlserver" {
  name                         = "twitpostersql${var.environment}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "sqladmin"
  administrator_login_password = "P@ssw0rd1234!"
  minimum_tls_version          = "1.2"
}

resource "azurerm_mssql_firewall_rule" "appServiceIP" {
  name                = "AllowAccessFromAzure"
  server_id           = azurerm_mssql_server.sqlserver.id
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

resource "azurerm_mssql_database" "sqldatabase" {
  name      = "twitposterdb${var.environment}"
  server_id = azurerm_mssql_server.sqlserver.id
  sku_name  = "S0"
}

locals {
  sql_connection_string = <<EOF
Server=tcp:${azurerm_mssql_server.sqlserver.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.sqldatabase.name};Persist Security Info=False;User ID=${azurerm_mssql_server.sqlserver.administrator_login};Password=${azurerm_mssql_server.sqlserver.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
EOF
}

resource "azurerm_service_plan" "asp" {
  name                = "twitposter-${var.environment}-asp"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "F1"

  depends_on = [
    azurerm_service_plan.function_app_plan,
    azurerm_linux_function_app.functionapp
  ]
}


# Define Application Insights
resource "azurerm_application_insights" "function_app_insights" {
  name                = "functionapp-appinsights-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.analyticsWorkspace.id
}

resource "azurerm_linux_function_app" "functionapp" {
  name                       = "twitposter-${var.environment}-functionapp"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  service_plan_id           = azurerm_service_plan.function_app_plan.id
  storage_account_name       = azurerm_storage_account.storage.name
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key

  site_config {
    application_stack {
      dotnet_version = "8.0"
      use_dotnet_isolated_runtime = true
    }
  }

  app_settings = {
    "AzureWebJobsStorage"               = "DefaultEndpointsProtocol=https;AccountName=${azurerm_storage_account.storage.name};AccountKey=${azurerm_storage_account.storage.primary_access_key};EndpointSuffix=core.windows.net"
  }

  identity {
    type = "SystemAssigned"
  }

  https_only = true

  lifecycle {
    ignore_changes = [
      app_settings["AzureWebJobsStorage"],
    ]
  }
}

resource "azurerm_linux_web_app" "appservice" {
  name                = "twitposter-${var.environment}-appservice"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.asp.id

  site_config {
    always_on = false
  }

  app_settings = {
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.appinsights.connection_string
    "ConnectionStrings__DbConnection"        = local.sql_connection_string
    "ConnectionStrings__ServiceBus"          = azurerm_servicebus_namespace.sbnamespace.default_primary_connection_string
    "Storage__AccountName"                   = azurerm_storage_account.storage.name
    "Storage__SharedKey"                     = azurerm_storage_account.storage.primary_access_key
    "Storage__Uri"                           = azurerm_storage_account.storage.primary_blob_endpoint
    "Secrets__UseSecrets"                    = true
    "Secrets__KeyVaultUri"                   = azurerm_key_vault.example_kv.vault_uri
  }

  identity {
    type = "SystemAssigned"
  }

  client_affinity_enabled = false
  https_only              = true
  lifecycle {
    ignore_changes = [
      app_settings["ConnectionStrings__DbConnection"],
    ]
  }
}

data "azurerm_client_config" "current" {}


resource "azurerm_key_vault" "example_kv" {
  name                        = "kv-tps-${var.environment}"
  location                    = azurerm_resource_group.rg.location
  resource_group_name         = azurerm_resource_group.rg.name
  sku_name                    = "standard"
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false


  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "Get"
    ]

    secret_permissions = [
      "Get", "Set", "List", "Delete"
    ]
  }
  
}




# Add an access policy for the managed identity of the App Service
resource "azurerm_key_vault_access_policy" "appservice_access_policy" {
  key_vault_id = azurerm_key_vault.example_kv.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_linux_web_app.appservice.identity[0].principal_id

  secret_permissions = [
    "Get",
    "List",
  ]

}
resource "random_password" "auth_secret" {
  length  = 32
  special = true
}


resource "azurerm_key_vault_secret" "auth_secret" {
  name         = "Auth--Secret"
  value        = random_password.auth_secret.result
  key_vault_id = azurerm_key_vault.example_kv.id
}


resource "azurerm_linux_web_app" "emailsender" {
  name                = "twitposter-${var.environment}-emailsender"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.asp.id

  site_config {
    always_on = false
  }

  app_settings = {
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.appinsights_emailsender.connection_string
    "ConnectionStrings__ServiceBus"          = azurerm_servicebus_namespace.sbnamespace.default_primary_connection_string
    "Mail__AuthPassword"          =       "bbwpcbswnsbupbmw"
  }

  identity {
    type = "SystemAssigned"
  }

  client_affinity_enabled = false
  https_only              = true
}

output "app_service_name" {
  value = azurerm_linux_web_app.appservice.name
}

output "functions_app_service_name" {
  value = azurerm_linux_function_app.functionapp.name
}

output "app_service_default_hostname" {
  value = azurerm_linux_web_app.appservice.default_hostname
}

output "publish_profile_command" {
  value       = "az webapp deployment list-publishing-profiles --name ${azurerm_linux_web_app.appservice.name} --resource-group ${azurerm_resource_group.rg.name} --xml"
  description = "Run this command in your shell to retrieve the Azure Web App's publishing profile."
}

output "emailsender_app_service_name" {
  value = azurerm_linux_web_app.emailsender.name
}

output "emailsender_app_service_default_hostname" {
  value = azurerm_linux_web_app.emailsender.default_hostname
}

output "emailsender_publish_profile_command" {
  value       = "az webapp deployment list-publishing-profiles --name ${azurerm_linux_web_app.emailsender.name} --resource-group ${azurerm_resource_group.rg.name} --xml"
  description = "Run this command in your shell to retrieve the Azure Email Sender Web App's publishing profile."
}

output "function_app_publish_profile_command" {
  value       = "az functionapp deployment list-publishing-profiles --name ${azurerm_linux_function_app.functionapp.name} --resource-group ${azurerm_resource_group.rg.name} --xml"
  description = "Run this command in your shell to retrieve the Azure Function App's publishing profile."
}