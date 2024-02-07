#! /bin/bash

set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname template1 <<-EOSQL
    CREATE EXTENSION IF NOT EXISTS pgstattuple;
    CREATE EXTENSION IF NOT EXISTS pg_stat_statements;
    CREATE EXTENSION IF NOT EXISTS pg_buffercache;

    -- todo pass over env
    CREATE USER example WITH PASSWORD 'example';
    CREATE DATABASE example /*TEMPLATE template1*/;
    GRANT ALL PRIVILEGES ON DATABASE example TO example;
EOSQL
