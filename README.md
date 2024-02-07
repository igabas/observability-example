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

## Troubleshooting

On MacOS while executing init scripts happens error:

> /usr/local/bin/docker-entrypoint.sh: running /docker-entrypoint-initdb.d/0001-init.sh\
> /usr/local/bin/docker-entrypoint.sh: /docker-entrypoint-initdb.d/0001-init.sh: /bin/bash: bad interpreter: Permission denied

For fix this needs to add execute permission for scripts:

```shell
find ./configs/postgres/scripts -type f \( -iname \*.sh -o -iname \*.sql \) -exec chmod +x {} +
```
