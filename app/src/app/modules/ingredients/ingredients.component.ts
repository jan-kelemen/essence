import { Component, OnInit } from '@angular/core';

import { IngredientsService } from './ingredients.service';
import { IngredientHeader } from './models/ingredient-header.model';

@Component({
  selector: 'app-ingredients',
  templateUrl: './ingredients.component.html',
  styleUrls: ['./ingredients.component.css']
})
export class IngredientsComponent implements OnInit {
  ingredients: IngredientHeader[] = []

  constructor(private ingredientsService: IngredientsService) {}

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }
}
