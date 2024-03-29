import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TextFieldModule } from '@angular/cdk/text-field';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';

import { IngredientsRoutingModule } from './ingredients-routing.module';
import { AddIngredientComponent } from './components/add-ingredient/add-ingredient.component';
import { EditIngredientComponent } from './components/edit-ingredient/edit-ingredient.component';
import { IngredientsComponent } from './ingredients.component';
import { IngredientDetailsComponent } from './components/ingredient-details/ingredient-details.component';

@NgModule({
  declarations: [
    AddIngredientComponent,
    EditIngredientComponent,
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
    MatCardModule,
    MatListModule
  ]
})
export class IngredientsModule { }
