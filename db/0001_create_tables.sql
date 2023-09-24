CREATE SCHEMA IF NOT EXISTS essence;

CREATE TABLE IF NOT EXISTS essence.ingredients
(
    id uuid NOT NULL CONSTRAINT ingredients_pk PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(250) NOT NULL CONSTRAINT ingredients_name_uq UNIQUE,
    summary VARCHAR(500),
    description VARCHAR(20000)
);

CREATE TABLE IF NOT EXISTS essence.recipes
(
    id uuid NOT NULL CONSTRAINT recipes_pk PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL CONSTRAINT recipes_name_uq UNIQUE,
    description VARCHAR(20000)
);