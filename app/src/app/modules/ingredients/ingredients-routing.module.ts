import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { IngredientsComponent } from './ingredients.component';
import { AddIngredientComponent } from './components/add-ingredient/add-ingredient.component';
import { EditIngredientComponent } from './components/edit-ingredient/edit-ingredient.component';
import { IngredientDetailsComponent } from './components/ingredient-details/ingredient-details.component';

const routes: Routes = [
  { path: '', component: IngredientsComponent },
  { path: 'add', component: AddIngredientComponent },
  { path: 'edit/:id', component: EditIngredientComponent },
  { path: 'details/:id', component: IngredientDetailsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IngredientsRoutingModule { }
