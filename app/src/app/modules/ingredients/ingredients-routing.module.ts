import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AddIngredientComponent } from './components/add-ingredient/add-ingredient.component';

const routes: Routes = [
  { path: '', redirectTo: 'add', pathMatch: 'full' },
  { path: 'add', component: AddIngredientComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IngredientsRoutingModule { }
