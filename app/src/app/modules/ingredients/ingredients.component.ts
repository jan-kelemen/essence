import { Component } from '@angular/core';

import { IngredientsService } from './ingredients.service';
import { IngredientHeader } from './models/ingredient-header.model';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-ingredients',
  templateUrl: './ingredients.component.html',
  styleUrls: ['./ingredients.component.css']
})
export class IngredientsComponent {
  list = [];
  listSubject$ = new Subject<IngredientHeader[]>();
  ingredients$ = this.listSubject$.asObservable();

  constructor(private ingredientsService: IngredientsService) {
    this.ingredients$ = ingredientsService.queryIngredients('');
  }

  onDelete(id: string): void {
    this.ingredientsService.deleteIngredient(id).subscribe(x => this.ingredients$ = this.ingredientsService.queryIngredients(''));
  }
}
