import { Component } from '@angular/core';

import { IngredientsService } from '../../ingredients.service';
import { NewIngredient } from '../../models/new-ingredient.model';

import { Router } from '@angular/router';

@Component({
  selector: 'app-add-ingredient',
  templateUrl: './add-ingredient.component.html',
  styleUrls: ['./add-ingredient.component.css']
})
export class AddIngredientComponent {

  constructor(private router: Router, private ingredientsService: IngredientsService) {
  }

  add(name: string, summary?: string, description?: string): void {
    name = name.trim();
    summary = summary?.trim();
    description = description?.trim();

    if (!name) { return; }

    this.ingredientsService.addIngredient({ name, summary, description } as NewIngredient)
      .subscribe((header) => { this.router.navigate([`/ingredients/details/${header.id}`]); });
  }
}
