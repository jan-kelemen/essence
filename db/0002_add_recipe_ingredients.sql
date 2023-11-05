CREATE TABLE IF NOT EXISTS essence.recipe_ingredient
(
    recipe_id uuid NOT NULL CONSTRAINT recipe_ingredient_recipe_fk REFERENCES essence.recipes(id),
    ingredient_id uuid NOT NULL CONSTRAINT recipe_ingredient_ingredient_fk REFERENCES essence.ingredients(id),
    amount integer NOT NULL CONSTRAINT recipe_ingredeint_amount_positive_chk CHECK(amount > 0),
    CONSTRAINT recipe_ingredients_pk PRIMARY KEY (recipe_id, ingredient_id)
);
