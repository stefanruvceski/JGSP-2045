import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';

import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Line, TimeTable } from './Model';
import { strictEqual } from 'assert';



const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({ providedIn: 'root' })
export class TimetableService {
  private TimeTableUrl = 'http://localhost:52295/api/lines';  // URL to web api
  private TimeTableUrl2 = 'http://localhost:52295/api/timetables';

  constructor(private http: HttpClient) { }

  /** GET hero by id. Will 404 if id not found */
  getLines(lineType: string): Observable<Line[]> {
    const url = `${this.TimeTableUrl}?lineType=${lineType}`;
    return this.http.get<Line[]>(url).pipe(
      catchError(this.handleError<Line[]>(`getHero id=${lineType}`))
    );
  }
  setTimetable(timetable: TimeTable): Observable<string> {
    return this.http.post<string>(this.TimeTableUrl2 + '/settimetable', timetable).pipe(catchError(this.errorHandler));
  }

  deleteTimetable(lineId: string, day: string): Observable<string> {
    const url = `${this.TimeTableUrl2}/DeleteTimetable?lineid=${lineId}&day=${day}`;
    return this.http.get<string>(url).pipe(
      catchError(this.handleError<string>(`getLine id=${lineId}`))
    );
  }

  getTimetable(LineId: string, day: string): Observable<string[]> {
    const url = `${this.TimeTableUrl2}?LineId=${LineId}&Day=${day}`;
    return this.http.get<string[]>(url).pipe(
      catchError(this.handleError<string[]>(`getHero id=${LineId}`))
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
