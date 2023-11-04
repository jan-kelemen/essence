import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IngredientsRoutingModule } from './ingredients-routing.module';
import { AddIngredientComponent } from './components/add-ingredient/add-ingredient.component';

@NgModule({
  declarations: [
    AddIngredientComponent
  ],
  imports: [
    CommonModule,
    IngredientsRoutingModule
  ]
})
export class IngredientsModule { }
