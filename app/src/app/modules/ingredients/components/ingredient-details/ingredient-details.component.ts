import { Component, OnInit } from '@angular/core';

import { IngredientsService } from '../../ingredients.service';
import { ActivatedRoute } from '@angular/router';
import { Ingredient } from '../../models/ingredient.model';

@Component({
  selector: 'app-ingredient-details',
  templateUrl: './ingredient-details.component.html',
  styleUrls: ['./ingredient-details.component.css']
})
export class IngredientDetailsComponent implements OnInit {
  ingredient: Ingredient | undefined;

  constructor(private route: ActivatedRoute, private ingredientService: IngredientsService) {}

  ngOnInit(): void {
    const routeParams = this.route.snapshot.paramMap;
    const ingredientId = routeParams.get('ingredientId');
  
    this.ingredientService.getIngredient(ingredientId!)
      .subscribe(i => this.ingredient = i);
  }
}
