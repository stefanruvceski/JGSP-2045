import { Component, OnInit, Input } from '@angular/core';
import { UserTicket } from '../Model';
import { TicketService } from '../ticket.service';


@Component({
  selector: 'app-user-tickets',
  templateUrl: './user-tickets.component.html',
  styleUrls: ['./user-tickets.component.scss']
})
export class UserTicketsComponent implements OnInit {
  @Input() tickets: UserTicket[];
  constructor(private ticketService: TicketService) { }

  ngOnInit() {
    this.getTickets();
  }

  getTickets() {
    this.ticketService.getTickets(localStorage.getItem('username')).subscribe(tickets => this.tickets = tickets);
  }

}
