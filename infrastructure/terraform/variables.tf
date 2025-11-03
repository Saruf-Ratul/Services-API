variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
  default     = "services-rg"
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "East US"
}

variable "cluster_name" {
  description = "AKS cluster name"
  type        = string
  default     = "services-aks"
}

variable "dns_prefix" {
  description = "DNS prefix for AKS"
  type        = string
  default     = "services"
}

variable "node_count" {
  description = "Number of nodes"
  type        = number
  default     = 3
}

variable "vm_size" {
  description = "VM size for nodes"
  type        = string
  default     = "Standard_D2s_v3"
}

variable "redis_cache_name" {
  description = "Redis cache name"
  type        = string
  default     = "services-redis"
}

variable "redis_capacity" {
  description = "Redis capacity"
  type        = number
  default     = 1
}

variable "redis_family" {
  description = "Redis family"
  type        = string
  default     = "C"
}

variable "redis_sku" {
  description = "Redis SKU"
  type        = string
  default     = "Standard"
}

variable "sql_server_name" {
  description = "SQL Server name"
  type        = string
  default     = "services-sqlserver"
}

variable "sql_admin_login" {
  description = "SQL Server admin login"
  type        = string
}

variable "sql_admin_password" {
  description = "SQL Server admin password"
  type        = string
  sensitive   = true
}

variable "sql_database_name" {
  description = "SQL Database name"
  type        = string
  default     = "Mobilize"
}

variable "sql_edition" {
  description = "SQL Database edition"
  type        = string
  default     = "Standard"
}

variable "sql_service_objective" {
  description = "SQL Database service objective"
  type        = string
  default     = "S2"
}

