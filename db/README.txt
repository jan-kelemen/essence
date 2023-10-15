Run the following commands to recreate the database:
* psql -U postgres -f 0000_create_user_and_database.sql
* psql -U essence -d essencepg -f 0001_create_tables.sql
* psql -U essence -d essencepg -f 0002_add_recipe_ingredients.sql
* psql -U essence -d essencepg -f 0003_increase_recipe_column_lengths.sql