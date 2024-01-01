import { Component, OnInit } from '@angular/core';

import { ActivatedRoute } from '@angular/router';
import { Observable, switchMap, take } from 'rxjs';

import { IngredientsService } from '../../ingredients.service';
import { Ingredient } from '../../models/ingredient.model';

@Component({
  selector: 'app-ingredient-details',
  templateUrl: './ingredient-details.component.html',
  styleUrls: ['./ingredient-details.component.css']
})
export class IngredientDetailsComponent {
  ingredient$: Observable<Ingredient>;

  constructor(
    private route: ActivatedRoute, 
    private ingredientsService: IngredientsService) {

    this.ingredient$ = this.route.params.pipe(
      take(1),
      switchMap(params => {
        const ingredientId = params['ingredientId'];
        return this.ingredientsService.getIngredient(ingredientId);
      }));
  }
}
