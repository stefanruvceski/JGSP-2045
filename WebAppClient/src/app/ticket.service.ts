import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';

import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Ticket, TicketVerification, UserTicket } from './Model';


const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({ providedIn: 'root' })
export class TicketService {
  private TicketsUrl = 'http://localhost:52295/api/ticket';  // URL to web api

  constructor(private http: HttpClient) { }

  /** GET hero by id. Will 404 if id not found */
  getTicket(ticketname: string): Observable<Ticket> {
    const url = `${this.TicketsUrl}/buyticket?username=${localStorage.getItem('username')}&ticketname=${ticketname}`;
    return this.http.get<Ticket>(url).pipe(
      catchError(this.handleError<Ticket>(`getTicket id=${ticketname}`))
    );
  }

  getTickets(username: string): Observable<UserTicket[]> {
    const url = `${this.TicketsUrl}/getusertickets?username=${username}`;
    return this.http.get<UserTicket[]>(url).pipe(
      catchError(this.handleError<UserTicket[]>(`getTickets id=${username}`))
    );
  }

  getTicketStatus(ticketId: string): Observable<TicketVerification> {
    const url = `${this.TicketsUrl}/getticketstatus?id=${ticketId}`;
    return this.http.get<TicketVerification>(url).pipe(
      catchError(this.handleError<TicketVerification>(`getTicketStatus id=${ticketId}`))
    );
  }

  setTicket(ticket: Ticket): Observable<Ticket> {
    console.log('confirm ticket....');
    console.log(this.TicketsUrl + '/confirmticket');
    console.log(ticket);
    return this.http.post<Ticket>(this.TicketsUrl + '/confirmticket', ticket).pipe(catchError(this.errorHandler));
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
