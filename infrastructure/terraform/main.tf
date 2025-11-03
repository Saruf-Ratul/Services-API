terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.0"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "services" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_kubernetes_cluster" "services" {
  name                = var.cluster_name
  location            = azurerm_resource_group.services.location
  resource_group_name = azurerm_resource_group.services.name
  dns_prefix          = var.dns_prefix

  default_node_pool {
    name       = "default"
    node_count = var.node_count
    vm_size    = var.vm_size
  }

  identity {
    type = "SystemAssigned"
  }
}

resource "azurerm_redis_cache" "services" {
  name                = var.redis_cache_name
  location            = azurerm_resource_group.services.location
  resource_group_name = azurerm_resource_group.services.name
  capacity            = var.redis_capacity
  family              = var.redis_family
  sku_name            = var.redis_sku
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"
}

resource "azurerm_sql_server" "services" {
  name                         = var.sql_server_name
  resource_group_name          = azurerm_resource_group.services.name
  location                     = azurerm_resource_group.services.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_login
  administrator_login_password = var.sql_admin_password
}

resource "azurerm_sql_database" "services" {
  name                = var.sql_database_name
  resource_group_name = azurerm_resource_group.services.name
  server_name         = azurerm_sql_server.services.name
  edition             = var.sql_edition
  requested_service_objective_name = var.sql_service_objective
}

