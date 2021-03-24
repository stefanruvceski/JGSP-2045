import { Component, OnInit, Input } from '@angular/core';
import { PricelistService } from '../pricelist.service';
import { Pricelist, PriceListAdmin } from '../Model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DatepickerOptions } from 'ng2-datepicker';

@Component({
  selector: 'app-admin-pricelist',
  templateUrl: './admin-pricelist.component.html',
  styleUrls: ['./admin-pricelist.component.scss']
})
export class AdminPricelistComponent implements OnInit {
  @Input() pricelistRegular: Pricelist[];
  @Input() pricelistStudent: Pricelist[];
  @Input() pricelistPensioner: Pricelist[];

  pricelist = new  PriceListAdmin('', '', '', '', '', '');
  feedback: string;

  expireOptions: DatepickerOptions = {
    minYear: 2020,
    maxYear: 2030,
    // position: 'top-right',
    displayFormat: 'MMM D[,] YYYY',
    barTitleFormat: 'MMMM YYYY',
    dayNamesFormat: 'dd',
    firstCalendarDay: 1, // 0 - Sunday, 1 - Monday
    // locale: frLocale,
     minDate: new Date(Date.now()), // Minimal selectable date
     maxDate: new Date(2030, 12, 31),  // Maximal selectable date
    barTitleIfEmpty: 'Click to select a date',
    placeholder: 'expire date', // HTML input placeholder attribute (default: '')
    addClass: 'form-control', // Optional, value to pass on to [ngClass] on the input field
    addStyle: {}, // Optional, value to pass to [ngStyle] on the input field
    fieldId: 'my-date-pickeri', // ID to assign to the input field. Defaults to datepicker-<counter>
    useEmptyBarTitle: false, // Defaults to true. If set to false then barTitleIfEmpty will be disregarded and
  };

  issueOptions: DatepickerOptions = {
    minYear: 2019,
    maxYear: 2030,
    // position: 'top-left',
    displayFormat: 'MMM D[,] YYYY',
    barTitleFormat: 'MMMM YYYY',
    dayNamesFormat: 'dd',
    firstCalendarDay: 1, // 0 - Sunday, 1 - Monday
    // locale: frLocale,
     minDate: new Date(Date.now()), // Minimal selectable date
     maxDate: new Date(this.pricelist.ExpireDate),  // Maximal selectable date
    barTitleIfEmpty: 'Click to select a date',
    placeholder: 'Issuing date', // HTML input placeholder attribute (default: '')
    addClass: 'form-control', // Optional, value to pass on to [ngClass] on the input field
    addStyle: {}, // Optional, value to pass to [ngStyle] on the input field
    fieldId: 'my-date-picker', // ID to assign to the input field. Defaults to datepicker-<counter>
    useEmptyBarTitle: false, // Defaults to true. If set to false then barTitleIfEmpty will be disregarded and
  };




  constructor(private pricelistService: PricelistService,  private modalService: NgbModal) { }

  ngOnInit() {

    this.getPricelist();
  }

  getPricelist(): void {
    this.pricelistService.getPricelist(1).subscribe(pricelists => this.pricelistRegular = pricelists);
    this.pricelistService.getPricelist(2).subscribe(pricelists => this.pricelistStudent = pricelists);
    this.pricelistService.getPricelist(3).subscribe(pricelists => this.pricelistPensioner = pricelists);
  }

  openWindowCustomClass(content) {
    this.modalService.open(content, { windowClass: 'dark-modal' });
  }

  addpricelist(content) {
    this.modalService.open(content, { windowClass: 'dark-modal' });
  }

  addnewpricelist(content) {
    this.pricelistService.addPricelist(this.pricelist).subscribe(data => {
      this.feedback = 'Successfuly added pricelis';
      this.openWindowCustomClass(content);
    }, error => {
      this.feedback = 'Unsuccessfuly added pricelis';
      this.openWindowCustomClass(content);
    });
  }

  ok() {
    this.modalService.dismissAll();
  }

  onKeyAnnual(event) {
    this.pricelist.AnnualTicketPrice = event.target.value;
  }

  onKeyTime(event) {
    this.pricelist.TimeTicketPrice = event.target.value;
  }

  onKeyDay(event) {
    this.pricelist.DailyTicketPrice = event.target.value;
  }

  onKeyMonth(event) {
    this.pricelist.MonthlyTicketPrice = event.target.value;
  }

  onchangeIssue() {
    if (this.pricelist.IssueDate) {
      const day = new Date(this.pricelist.IssueDate);
      this.expireOptions.minDate.setFullYear(day.getFullYear(), day.getMonth() + 3, day.getDate());
    }
  }

  onchangeExpire() {
    if (this.pricelist.ExpireDate) {
      const day = new Date(this.pricelist.ExpireDate);
      this.issueOptions.maxDate.setFullYear(day.getFullYear(), day.getMonth() - 3, day.getDate());
    }
  }

}

