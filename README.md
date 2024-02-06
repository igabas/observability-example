# Observability example

Демонстрационный проект "наблюдаемости" ASP.NET Core с использованием SDK [Open Telemetry](https://opentelemetry.io/).

___

## Установка

Для запуска проекта необходимы

- Docker
- .NET 8.0

___

## Запуск

Для запуска проекта используйте команду

```shell
docker compose up -d
```

После успешного запуска компоненты будут доступны по следующим адресам:

* http-service http://localhost:8081
* grpc-service http://localhost:8092 (grpc) и http://localhost:8091 (http)
* otel-collector http://localhost:8889/metrics
* Jaeger http://localhost:16686/
* Prometheus http://localhost:9090/
* Grafana http://localhost:3000/ (user: `admin`, password: `P@ssw0rd`)
