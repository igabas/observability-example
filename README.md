# Observability example
___

## Описание

Демонстрационный проект "наблюдаемости" ASP.NET Core с использованием SDK [Open Telemetry](https://opentelemetry.io/).

Состоит из:
* http-service
* grpc-service
* Бэкенд трейсинга [Jaeger](https://www.jaegertracing.io/docs/1.53/)
* Бэкенд метрик [Prometheus](https://prometheus.io/)
* GUI метрик [Grafana](https://grafana.com/)

___

## Запуск
Для запуска проекта используйте команду
```shell
docker compose up -d
```
После успешного запуска компоненты будут доступны по следующим адресам:
* Jaeger http://localhost:16686/
* Prometheus http://localhost:9090/
* Grafana http://localhost:3000/
* http-service http://localhost:8081
* grpc-service http://localhost:8092 (grpc) и http://localhost:8091 (http)

Для Grafana необходимо установить пароль для пользователя `admin`
, а также настроить [datasource Prometheus](http://localhost:3000/connections/datasources)
указав адрес `http://prometheus:9090`.

Также можно импортировать полезные борды для Grafana
* https://grafana.com/grafana/dashboards/17706-asp-net-otel-metrics/
* https://grafana.com/grafana/dashboards/19924-asp-net-core
* https://grafana.com/grafana/dashboards/19925-asp-net-core-endpoint/
