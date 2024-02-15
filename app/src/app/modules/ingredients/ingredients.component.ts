import { Component, OnInit } from '@angular/core';

import { IngredientsService } from './ingredients.service';
import { IngredientHeader } from './models/ingredient-header.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-ingredients',
  templateUrl: './ingredients.component.html',
  styleUrls: ['./ingredients.component.css']
})
export class IngredientsComponent implements OnInit {
  ingredients$: Observable<IngredientHeader[]>;

  constructor(private ingredientsService: IngredientsService) {
    this.ingredients$ = ingredientsService.queryIngredients('');
  }

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }
}
