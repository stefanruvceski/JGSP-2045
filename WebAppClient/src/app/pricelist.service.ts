import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';

import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Pricelist, PriceListAdmin } from './Model';
// export class Pricelist {
//   name: string;
//   price: number;
// }


const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({ providedIn: 'root' })
export class PricelistService {
  private PricelistesUrl = 'http://localhost:52295/api/pricelist';  // URL to web api

  constructor(private http: HttpClient) { }

  /** GET hero by id. Will 404 if id not found */
  getPricelist(ageGroupId: number): Observable<Pricelist[]> {
    const url = `${this.PricelistesUrl}?ageGroupId=${ageGroupId}`;
    return this.http.get<Pricelist[]>(url).pipe(
      catchError(this.handleError<Pricelist[]>(`getHero id=${ageGroupId}`))
    );
  }

  addPricelist(pricelist: PriceListAdmin): Observable<PriceListAdmin> {

    return this.http.post<PriceListAdmin>(this.PricelistesUrl + '/addpricelist', pricelist).pipe(catchError(this.errorHandler));
  }

  errorHandler(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      console.error('An error occurred: ', error.error.message);
    } else {
      console.error(
        `Backend returned code ${error.status}` + `body was: ${error.error.error_description}`
      );
    }

    return throwError('Something bad happend, please try again later...');
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      return of(result as T);
    };
  }


}
