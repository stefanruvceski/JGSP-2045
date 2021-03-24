import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TicketService } from '../ticket.service';
import { Ticket} from '../Model';
import { tick } from '@angular/core/src/render3';
import { Identifiers } from '@angular/compiler';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],

})
export class HomeComponent implements OnInit {
  imageUrlArray: string[];
  username: string;
  ticketType = 'Time Ticket';
  ticket: Ticket;
  status = 'info';
  statusString = 'Validate';
  role: string;
  id: number;
  constructor(private modalService: NgbModal, private ticketService: TicketService) { }

  ngOnInit() {

    this.username = localStorage.getItem('username');
    this.role = localStorage.getItem('role');


    this.imageUrlArray = ['https://www.richardvanhooijdonk.com/wp-content/uploads/2017/12/future-bus-1024x512-1.jpg',
                          'https://i.pinimg.com/originals/c8/19/44/c819441a554001f34a34965e0ed00eaf.jpg',
                          'https://www.goodnet.org/photos/620x0/27785_hd.png',
                          'https://www.mercedes-benz.com/wp-content/uploads/sites/3/2017/01/07-mercedes-benz-design-future-bus-680x379.jpg',
                          'https://static.turbosquid.com/Preview/001266/787/WT/_D.jpg'];
  }

  openWindowCustomClass(content) {
    this.modalService.open(content, { windowClass: 'dark-modal' });
  }

  ok() {
    this.ticket.Description = localStorage.getItem('username');
    this.ticketService.setTicket(this.ticket).subscribe();
    this.ticket.Description = 'Success';
   // this.modalService.dismissAll();
  }

  cancel() {
    this.ticket.Description = '';
    this.ticketService.setTicket(this.ticket);
    this.modalService.dismissAll();
  }

  buyTicket(content) {
    this.ticketService.getTicket(this.ticketType).subscribe(ticket =>
      this.ticket = ticket);
    this.modalService.dismissAll();
    this.openWindowCustomClass(content);
  }

  verificate(id: string) {
    this.ticketService.getTicketStatus(id).subscribe(ticket => {
      this.status = (ticket).Status;       // glupi ts, moze i bez ovog kastovanja (radi, ali pise error u terminalu)
      this.statusString = (ticket).Description;
    }
    );
  }

  onchange() {
    this.status = 'info';
    this.statusString = 'Validate';
  }
}
