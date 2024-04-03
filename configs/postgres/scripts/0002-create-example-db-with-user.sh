#! /bin/bash

set -e

# 1. create user and database
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE USER example WITH PASSWORD 'example';
    CREATE DATABASE example TEMPLATE template1;
    GRANT ALL PRIVILEGES ON DATABASE example TO example;
EOSQL

# 2. add privileges on default schema to user
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname example <<-EOSQL
    GRANT USAGE, CREATE ON SCHEMA public TO example;
EOSQL
