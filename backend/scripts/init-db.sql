-- Script de inicialización de la base de datos Trycore EVM.
-- Es el equivalente en SQL plano a las migraciones de EF Core (ver README.md
-- para generarlas y aplicarlas automáticamente con `dotnet ef database update`).
-- Se incluye aquí para poder levantar el esquema sin depender de la CLI de EF Core.

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS projects (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" varchar(200) NOT NULL
);

CREATE TABLE IF NOT EXISTS project_activities (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "ProjectId" uuid NOT NULL REFERENCES projects ("Id") ON DELETE CASCADE,
    "Name" varchar(200) NOT NULL,
    "BudgetAtCompletion" numeric(18, 2) NOT NULL,
    "PlannedPercentComplete" numeric(5, 2) NOT NULL,
    "ActualPercentComplete" numeric(5, 2) NOT NULL,
    "ActualCost" numeric(18, 2) NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_project_activities_project_id ON project_activities ("ProjectId");
