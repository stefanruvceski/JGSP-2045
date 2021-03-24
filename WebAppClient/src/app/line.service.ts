import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';

import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Ticket, TicketVerification, UserTicket, LineSt } from './Model';


const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({ providedIn: 'root' })
export class LineService {
  private TicketsUrl = 'http://localhost:52295/api/stationline';  // URL to web api

  constructor(private http: HttpClient) { }

  setLine(line: LineSt): Observable<LineSt> {
    return this.http.post<LineSt>(this.TicketsUrl + '/addnewline', line).pipe(catchError(this.errorHandler));
  }

  editLine(line: LineSt): Observable<LineSt> {
    return this.http.post<LineSt>(this.TicketsUrl + '/editline', line).pipe(catchError(this.errorHandler));
  }

  getLine(lineId: string): Observable<LineSt> {
    const url = `${this.TicketsUrl}/getline?lineid=${lineId}`;
    return this.http.get<LineSt>(url).pipe(
      catchError(this.handleError<LineSt>(`getLine id=${LineSt}`))
    );
  }

  deleteLine(lineId: string): Observable<LineSt> {
    const url = `${this.TicketsUrl}/deleteline?lineid=${lineId}`;
    return this.http.get<LineSt>(url).pipe(
      catchError(this.handleError<LineSt>(`getLine id=${LineSt}`))
    );
  }

  getLines(): Observable<string[]> {
    const url = `${this.TicketsUrl}/getlines`;
    return this.http.get<string[]>(url).pipe(
      catchError(this.handleError<string[]>(`getLine id=${LineSt}`))
    );
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
