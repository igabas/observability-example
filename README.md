# Observability example

Демонстрационный проект "наблюдаемости" ASP.NET Core с использованием SDK [Open Telemetry](https://opentelemetry.io/).

___

## Установка
Для запуска проекта необходимы   
- Docker
- .NET 8.0
- OpenSSL

___

## Запуск
Для запуска проекта используйте команду
```shell
docker compose up -d
```

После успешного запуска компоненты будут доступны по следующим адресам:
* Jaeger http://localhost:16686/
* Prometheus http://localhost:9090/
* Grafana http://localhost:3000/ (user: `admin`, password: `P@ssw0rd`)
* http-service http://localhost:8081
* grpc-service http://localhost:8092 (grpc) и http://localhost:8091 (http)

 

Также можно импортировать полезные борды для Grafana
* https://grafana.com/grafana/dashboards/17706-asp-net-otel-metrics/
* https://grafana.com/grafana/dashboards/19924-asp-net-core
* https://grafana.com/grafana/dashboards/19925-asp-net-core-endpoint/
