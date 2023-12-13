import { Component, ViewChild, NgZone } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

import { CdkTextareaAutosize } from '@angular/cdk/text-field';

import {take} from 'rxjs/operators';

import { IngredientsService } from '../../ingredients.service';
import { NewIngredient } from '../../models/new-ingredient.model';

@Component({
  selector: 'app-add-ingredient',
  templateUrl: './add-ingredient.component.html',
  styleUrls: ['./add-ingredient.component.css']
})
export class AddIngredientComponent {
  @ViewChild('autosize') autosize?: CdkTextareaAutosize;

  addIngredientForm = this.formBuilder.group({
    name: '',
    summary: '',
    description: ''
  });

  constructor(
    private router: Router, 
    private formBuilder: FormBuilder,
    private ingredientsService: IngredientsService,
    private _ngZone: NgZone) {
  }

  triggerResize() {
    // Wait for changes to be applied, then trigger textarea resize.
    this._ngZone.onStable.pipe(take(1)).subscribe(() => this.autosize!.resizeToFitContent(true));
  }

  onSubmit(): void {
    console.log('submitted');
    const name = this.addIngredientForm.value.name?.trim();
    const summary = this.addIngredientForm.value.summary?.trim();
    const description = this.addIngredientForm.value.description?.trim();

    console.log('submitted');
    if (!name) { return; }

    console.log('submitted');
    this.ingredientsService.addIngredient({ name, summary, description } as NewIngredient)
      .subscribe((header) => { this.router.navigate([`/ingredients/details/${header.id}`]); });
  }
}
