output "kubernetes_cluster_name" {
  value = azurerm_kubernetes_cluster.services.name
}

output "kubernetes_cluster_fqdn" {
  value = azurerm_kubernetes_cluster.services.fqdn
}

output "redis_hostname" {
  value = azurerm_redis_cache.services.hostname
}

output "sql_server_fqdn" {
  value = azurerm_sql_server.services.fully_qualified_domain_name
}

