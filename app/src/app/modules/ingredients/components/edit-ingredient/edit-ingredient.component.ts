import { Component, ViewChild, NgZone } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { CdkTextareaAutosize } from '@angular/cdk/text-field';

import { Observable } from 'rxjs';
import { take, switchMap, tap} from 'rxjs/operators';

import { IngredientsService } from '../../ingredients.service';
import { Ingredient } from '../../models/ingredient.model';

@Component({
  selector: 'app-edit-ingredient',
  templateUrl: './edit-ingredient.component.html',
  styleUrls: ['./edit-ingredient.component.css']
})
export class EditIngredientComponent {
  ingredient$: Observable<Ingredient>;

  @ViewChild('autosize') autosize?: CdkTextareaAutosize;

  ingredientForm = this.formBuilder.group({
    id: '',
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
 
    this.ingredient$ = this.route.params.pipe(
      take(1),
      switchMap(params => {
        const ingredientId = params['id'];
        return this.ingredientsService.getIngredient(ingredientId);
      }),
      tap(ingredient => {
        this.ingredientForm.patchValue({
          id: ingredient.id,
          name: ingredient.name,
          summary: ingredient.summary,
          description: ingredient.description
        })
      })
    );
  }

  triggerResize() {
    this._ngZone.onStable.pipe(take(1)).subscribe(() => this.autosize!.resizeToFitContent(true));
  }

  onSubmit(): void {
    const id = this.ingredientForm.value.id?.trim();
    const name = this.ingredientForm.value.name?.trim();
    const summary = this.ingredientForm.value.summary?.trim();
    const description = this.ingredientForm.value.description?.trim();

    if (!name) { return; }

    this.ingredientsService.updateIngredient({ id, name, summary, description } as Ingredient)
      .subscribe((header) => { this.router.navigate([`/ingredients/details/${id}`]); });
  }
}
