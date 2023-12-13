import { Component, ViewChild, NgZone } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { CdkTextareaAutosize } from '@angular/cdk/text-field';

import { take } from 'rxjs/operators';

import { IngredientsService } from '../../ingredients.service';
import { Ingredient } from '../../models/ingredient.model';

@Component({
  selector: 'app-edit-ingredient',
  templateUrl: './edit-ingredient.component.html',
  styleUrls: ['./edit-ingredient.component.css']
})
export class EditIngredientComponent {
  ingredient: Ingredient | undefined;

  @ViewChild('autosize') autosize?: CdkTextareaAutosize;

  editIngredientForm = this.formBuilder.group({
    name: '',
    summary: '',
    description: ''
  });

  constructor(
    private route: ActivatedRoute,
    private router: Router, 
    private formBuilder: FormBuilder,
    private ingredientsService: IngredientsService,
    private _ngZone: NgZone) {

    const routeParams = this.route.snapshot.paramMap;
    const ingredientId = routeParams.get('ingredientId');
  
    this.ingredientsService.getIngredient(ingredientId!)
      .subscribe(i => {
        this.ingredient = i;

        this.editIngredientForm.setValue({
          name: i!.name,
          summary: i!.summary!,
          description:  i!.description!,
        })
      });
  }

  triggerResize() {
    this._ngZone.onStable.pipe(take(1)).subscribe(() => this.autosize!.resizeToFitContent(true));
  }

  onSubmit(): void {
    const name = this.editIngredientForm.value.name?.trim();
    const summary = this.editIngredientForm.value.summary?.trim();
    const description = this.editIngredientForm.value.description?.trim();

    if (!name) { return; }

    this.ingredientsService.updateIngredient({ id: this.ingredient!.id, name, summary, description } as Ingredient)
      .subscribe((header) => { this.router.navigate([`/ingredients/details/${this.ingredient!.id}`]); });
  }
}
