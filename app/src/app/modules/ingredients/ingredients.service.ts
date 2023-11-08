import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { IngredientHeader } from './models/ingredient-header.model';
import { Ingredient } from './models/ingredient.model';
import { NewIngredient } from './models/new-ingredient.model';

@Injectable({
  providedIn: 'root'
})
export class IngredientsService {

  private ingredientsUrl = 'http://localhost:5011/api/Ingredient';

  private httpOptions = {
    headers: new HttpHeaders(
      { 'Content-Type': 'application/json', 'Access-Control-Allow-Origin': '*' })
  };

  constructor(private http: HttpClient) { }

  getIngredient(id: string): Observable<Ingredient> {
    const action = 'GetIngredient';
    const url = `${this.ingredientsUrl}/${action}/${id}`;

    return this.http.get<Ingredient>(url, this.httpOptions)
      .pipe(catchError(this.handleError<Ingredient>(action)));
  }

  queryIngredients(prefix?: string): Observable<IngredientHeader[]> {
    const action = 'QueryIngredients';

    let params = new HttpParams();
    if (prefix != null) {
      params.append("prefix", prefix);
    }

    return this.http.get<IngredientHeader[]>(`${this.ingredientsUrl}/${action}`, { params, ...this.httpOptions })
      .pipe(catchError(this.handleError<IngredientHeader[]>(action, [])));
  }

  addIngredient(ingredient: NewIngredient): Observable<IngredientHeader> {
    const action = 'AddIngredient';
    const url = `${this.ingredientsUrl}/${action}`;

    const body = {
      name: ingredient.name,
      summary: ingredient.summary,
      description: ingredient.description
    };

    return this.http.put<IngredientHeader>(url, body, this.httpOptions)
      .pipe(catchError(this.handleError<IngredientHeader>(action)));
  }

  updateIngredient(ingredient: Ingredient): Observable<any> {
    const action = 'UpdateIngredient';

    const body = {
      id: ingredient.id,
      name: ingredient.name,
      summary: ingredient.summary,
      description: ingredient.description,
    };

    return this.http.post(`${this.ingredientsUrl}/${action}`, body, this.httpOptions)
      .pipe(catchError(this.handleError<any>(action)));
  }

  deleteIngredient(id: string): Observable<any> {
    const action = 'DeleteIngredient';
    const url = `${this.ingredientsUrl}/${action}/${id}`;

    return this.http.delete(url)
      .pipe(catchError(this.handleError<any>(action)));
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} error: ${error}`);
      return of(result as T);
    };
  }
}
