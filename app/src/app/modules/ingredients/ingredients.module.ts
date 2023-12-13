import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CdkTextareaAutosize, TextFieldModule } from '@angular/cdk/text-field';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';

import { IngredientsRoutingModule } from './ingredients-routing.module';
import { AddIngredientComponent } from './components/add-ingredient/add-ingredient.component';
import { IngredientsComponent } from './ingredients.component';
import { IngredientDetailsComponent } from './components/ingredient-details/ingredient-details.component';

@NgModule({
  declarations: [
    AddIngredientComponent,
    IngredientsComponent,
    IngredientDetailsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IngredientsRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
  ]
})
export class IngredientsModule { }
