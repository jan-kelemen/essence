CREATE SCHEMA essence;

CREATE TABLE essence.recipes (
	id uuid DEFAULT gen_random_uuid() PRIMARY KEY,
	name VARCHAR (50) UNIQUE NOT NULL,
	description VARCHAR (256)
);
