import { Component, OnInit, NgZone } from '@angular/core';
import { LineSt } from '../Model';
import { LineService } from '../line.service';
import { Validators, FormBuilder } from '@angular/forms';
import { forEach } from '@angular/router/src/utils/collection';
import { Alert } from 'selenium-webdriver';

@Component({
  selector: 'app-line-network',
  templateUrl: './line-network.component.html',
  styleUrls: ['./line-network.component.scss'],
  styles: ['agm-map {height: 500px; width: 100%;}']
})
export class LineNetworkComponent implements OnInit {
  lines: Array<LineSt>;
  lineIds: Array<string>;
  map = new Map<string, LineSt>();
  iconUrl = { url: 'assets/busicon.png', scaledSize: {width: 30, height: 30}};
  profileForm = this.fb.group({
    lineNumber: ['', [Validators.required, Validators.pattern('[0-9]+(A|B|a|b)')]],
    lineType: ['', Validators.required] ,
    description: [''],
    color: ['blue']
    });

  constructor(private fb: FormBuilder, private ngZone: NgZone, private lineService: LineService) { }

  ngOnInit() {

    this.lines = new Array<LineSt>();
    this.lineService.getLines().subscribe(l =>  {
      this.lineIds = l;
    });

  }

  getLines(): Array<LineSt> {
    return Array.from( this.map.values() );
  }

  getLine(lineId: string) {

    this.lineService.getLine(lineId).subscribe(line => {

          this.map.set(line.LineId, line);

          console.log(this.map.get(line.LineId).Stations);
      });

  }
  someFunction(lineId: string, event) {
    if (event.target.checked) {
        this.getLine(lineId);
    } else {
        this.map.delete(lineId);
    }

  }

}
