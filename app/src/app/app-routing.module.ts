import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: 'ingredients', pathMatch: 'full' },
  {
    path: 'ingredients',
    loadChildren: () => import('./modules/ingredients/ingredients.module').then(m => m.IngredientsModule)
  },
  {
    path: 'recipes',
    loadChildren: () => import('./modules/recipes/recipes.module').then(m => m.RecipesModule)
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
