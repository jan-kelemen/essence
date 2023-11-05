Create the user and database with:
```
CREATE USER essence;
ALTER USER essence WITH PASSWORD 'essence';

CREATE DATABASE essencepg WITH owner = essence;
```

Run the following commands to recreate the database:
* psql -U essence -d essencepg -f 0001_create_tables.sql
* psql -U essence -d essencepg -f 0002_add_recipe_ingredients.sql
* psql -U essence -d essencepg -f 0003_increase_recipe_column_lengths.sql