import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AddIngredientComponent } from './components/add-ingredient/add-ingredient.component';
import { IngredientDetailsComponent } from './components/ingredient-details/ingredient-details.component';

const routes: Routes = [
  { path: '', redirectTo: 'add', pathMatch: 'full' },
  { path: 'add', component: AddIngredientComponent },
  { path: 'details/:ingredientId', component: IngredientDetailsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IngredientsRoutingModule { }
