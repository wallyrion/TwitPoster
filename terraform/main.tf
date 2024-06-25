terraform {
  backend "azurerm" {
    resource_group_name   = "tfstate-rg"
    storage_account_name  = "tfstate12345"
    container_name        = "tfstate"
    key                   = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "twitposter-${var.environment}-rg"
  location = "East US"
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

resource "azurerm_application_insights" "appinsights" {
  name                = "twitposter-ai-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

resource "azurerm_application_insights" "appinsights_emailsender" {
  name                = "twitposter-ai-emailsender-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

resource "azurerm_mssql_server" "sqlserver" {
  name                         = "twitpostersql${var.environment}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "sqladmin"
  administrator_login_password = "P@ssw0rd1234!"
  minimum_tls_version          = "1.2"

  tags = {
    environment = "production"
  }
}

resource "azurerm_mssql_database" "sqldatabase" {
  name      = "twitposterdb${var.environment}"
  server_id = azurerm_mssql_server.sqlserver.id
  sku_name  = "S0"
}


resource "azurerm_service_plan" "asp" {
  name                = "twitposter-${var.environment}-asp"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "F1"
}

locals {
  sql_connection_string = <<EOF
Server=tcp:${azurerm_mssql_server.sqlserver.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.sqldatabase.name};Persist Security Info=False;User ID=${azurerm_mssql_server.sqlserver.administrator_login};Password=${azurerm_mssql_server.sqlserver.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
EOF
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
    "Auth__Secret"                           = "mysupersecret_secretkey!123_for#TwitPosterApp"
    "ConnectionStrings__DbConnection"        = local.sql_connection_string
    "ConnectionStrings__ServiceBus"          = azurerm_servicebus_namespace.sbnamespace.default_primary_connection_string
    "Storage__AccountName"                   = azurerm_storage_account.storage.name
    "Storage__SharedKey"                     = azurerm_storage_account.storage.primary_access_key
    "Storage__Uri"                           = azurerm_storage_account.storage.primary_blob_endpoint
  }

  identity {
    type = "SystemAssigned"
  }

  client_affinity_enabled = false
  https_only              = true
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
    "Mail__AuthPassword"          = bbwpcbswnsbupbmw
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
