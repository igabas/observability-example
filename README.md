# Observability ASP.NET Core example

A simple observability project on ASP.NET Core using [Open Telemetry SDK](https://opentelemetry.io/)

## Prerequisites

- Docker
- .NET 8.0

## Running 🚀

Run project services use command:

```shell
docker compose up -d
```

or

```shell
docker-compose up -d --force-recreate --no-deps --build http-service grpc-service  
```

## Service`s addresses

| service           | url / port                                     | notes                                    | 
|-------------------|------------------------------------------------|------------------------------------------| 
| http-service      | [swager](http://localhost:8081/swagger)        |                                          | 
| grpc-service      | [grpc reflection](http://localhost:8092)       |                                          |  
| otel-collector    | [metrics path](http://localhost:8889/metrics)  |                                          |  
| postgreSQL        | port: 6432;                                    | setup via [.env](.//.env) file           |   
| postgres-exporter | [metrics path](http://localhost:9187/metrics)  |                                          |     
| Jaeger            | [Jaeger UI](http://localhost:16686)            |                                          |      
| Grafana           | [Grafana UI](http://localhost:3000/dashboards) | user/pass setup via [.env](.//.env) file |       
| Prometheus        | [Prometheus UI](http://localhost:9090)         |                                          |        
| kafka             | [Kafka](http://localhost:39092)                |                                          |
| akhq              | [Kafka UI](http://localhost:8099)              |                                          |
| kafka-exporter    | [metrics path](http://localhost:9308/metrics)  |                                          |

## Troubleshooting 🧐

On MacOS while executing init scripts happens error:

> /usr/local/bin/docker-entrypoint.sh: running /docker-entrypoint-initdb.d/0001-init.sh\
> /usr/local/bin/docker-entrypoint.sh: /docker-entrypoint-initdb.d/0001-init.sh: /bin/bash: bad interpreter: Permission
> denied

For fix this needs to add execute permission for scripts:

```shell
find ./configs/postgres/scripts -type f \( -iname \*.sh -o -iname \*.sql \) -exec chmod +x {} +
```
