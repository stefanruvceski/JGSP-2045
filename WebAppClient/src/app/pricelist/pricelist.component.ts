import { Component, OnInit, Input } from '@angular/core';
import {PricelistService} from '../pricelist.service';
import { Observable, from } from 'rxjs';
import { Pricelist } from '../Model';




@Component({
  selector: 'app-pricelist',
  templateUrl: './pricelist.component.html',
  styleUrls: ['./pricelist.component.scss']
})



export class PricelistComponent implements OnInit {
  @Input() pricelistRegular: Pricelist[];
  @Input() pricelistStudent: Pricelist[];
  @Input() pricelistPensioner: Pricelist[];
  constructor(private pricelistService: PricelistService) { }

  ngOnInit() {
    this.getPricelist();
  }

  getPricelist(): void {
    this.pricelistService.getPricelist(1).subscribe(pricelists => this.pricelistRegular = pricelists);
    this.pricelistService.getPricelist(2).subscribe(pricelists => this.pricelistStudent = pricelists);
    this.pricelistService.getPricelist(3).subscribe(pricelists => this.pricelistPensioner = pricelists);
  }

}
