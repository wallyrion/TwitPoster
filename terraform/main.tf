﻿provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "twitposter-${var.environment}-rg"
  location = "East US"
}

resource "azurerm_service_plan" "asp" {
  name                = "twitposter-${var.environment}-asp"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "F1"
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
    "Auth__Secret" = "mysupersecret_secretkey!123_for#TwitPosterApp"
  }

  identity {
    type = "SystemAssigned"
  }

  client_affinity_enabled = false
  https_only              = true
}

resource "null_resource" "fetch_publish_profile" {
  depends_on = [azurerm_linux_web_app.appservice]

  provisioner "local-exec" {
    command = <<EOT
      az webapp deployment list-publishing-profiles --name ${azurerm_linux_web_app.appservice.name} --resource-group ${azurerm_resource_group.rg.name} --xml > .terraform/publish_profile.xml
    EOT
  }
}

data "local_file" "publish_profile" {
  depends_on = [null_resource.fetch_publish_profile]
  filename   = "${path.module}/.terraform/publish_profile.xml"
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
