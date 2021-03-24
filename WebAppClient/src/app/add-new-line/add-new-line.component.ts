import { Component, OnInit, NgZone } from '@angular/core';
import { LineSt, Station, Line } from '../Model';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { FormBuilder, Validators } from '@angular/forms';
import { LineService } from '../line.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { stringify } from '@angular/core/src/util';

@Component({
  selector: 'app-add-new-line',
  templateUrl: './add-new-line.component.html',
  styleUrls: ['./add-new-line.component.scss'],
  styles: ['agm-map {height: 550px; width: 100%;}']
})
export class AddNewLineComponent implements OnInit {
  line: LineSt;
  feedback: string;
  styleString: string;
  newLine: LineSt;
  public zoom: number;
  httpClient: HttpClient;
  address: string;
  iconUrl = { url: 'assets/busicon.png', scaledSize: {width: 50, height: 50}};
  linenumber = 0;
  profileForm = this.fb.group({
    lineNumber: ['', [Validators.required, Validators.pattern('[0-9]+(A|B|a|b)')]],
    lineType: ['', Validators.required] ,
    description: [''],
    color: ['#ffffff']
    });
    flag: string;
    lineIds: Array<string>;
  constructor(private route: ActivatedRoute, private router: Router, private fb: FormBuilder, private ngZone: NgZone,
              private handler: HttpBackend, private lineService: LineService, private modalService: NgbModal) {
    this.httpClient = new  HttpClient(handler);
    this.flag = this.route.snapshot.paramMap.get('flag');


   }

  ngOnInit() {
    this.lineService.getLines().subscribe(l =>  {
      this.lineIds = l;
      this.styleString = 'display: inline-block; width:' + l.length * 40 + ' px; ';

      console.log(this.lineIds );
    });
    this.line = new LineSt('0', 'Urban', '', 'blue');
    this.flag = this.route.snapshot.paramMap.get('flag');

  }

  leftClick($event) {
    this.getCurrentLocation($event.coords.lat, $event.coords.lng, 'Yes');
  }

  rightClick($event) {
    this.getCurrentLocation($event.coords.lat, $event.coords.lng, 'No');
  }

  getCurrentLocation(lat: number, lng: number, isStation: string) {
// tslint:disable-next-line: max-line-length
     this.httpClient.get<any>(' https://maps.googleapis.com/maps/api/geocode/json?latlng=' + lat + ',' + lng + '&key=AIzaSyDnihJyw_34z5S1KZXp90pfTGAqhFszNJk').subscribe(data => {
        console.log(data);
        const result = data.results[0];
        let name = '';
        let address = '';
        if (result.address_components[0].long_name.includes('-')) {
          name = result.address_components[0].long_name;
          address =  address = result.address_components[1].long_name + ', ' + result.address_components[2].long_name;
        } else {
          address = result.address_components[1].long_name + ' ' +
                    result.address_components[0].long_name + ', ' + result.address_components[2].long_name;
          name = address;
        }

        const station = new Station(address, address, lat, lng, isStation);
        if (this.flag === 'add') {
          this.line.Stations.push(station);
        } else {
          this.newLine.Stations.push(station);
        }

        console.log(this.line);
     });

}
onDelete() {
  this.lineService.deleteLine(this.line.LineId).subscribe(data => window.location.reload());

}
openWindowCustomClass(content) {
  this.modalService.open(content, { windowClass: 'dark-modal' });
}
onSubmit(content) {

  if (this.flag === 'add') {
    this.line.LineId = this.profileForm.controls.lineNumber.value;
    this.line.LineType = this.profileForm.controls.lineType.value;
    this.line.Description = this.profileForm.controls.description.value;
    this.line.Color = this.profileForm.controls.color.value;
    this.lineService.setLine(this.line).subscribe(data => {
      this.feedback = 'Successfuly added line';
      this.openWindowCustomClass(content);
    }, error => {
      this.feedback = 'Unsuccessfuly added line';
      this.openWindowCustomClass(content);
    });
  } else {
    this.newLine.LineId = this.profileForm.controls.lineNumber.value;
    this.newLine.LineType = this.profileForm.controls.lineType.value;
    this.newLine.Description = this.profileForm.controls.description.value;
    this.newLine.Color = this.profileForm.controls.color.value;
    if (this.newLine.Stations.length === 0) {
      this.newLine.Stations = this.line.Stations;
    }
    this.lineService.editLine(this.newLine).subscribe(data => {
      this.feedback = 'Successfuly edited line';
      this.openWindowCustomClass(content);
    }, error => {
      this.feedback = 'Unsuccessfuly edited line';
      this.openWindowCustomClass(content);
    });
  }
  this.line = new LineSt('0', 'Urban', '', 'blue');

  this. profileForm = this.fb.group({
    lineNumber: ['', [Validators.required, Validators.pattern('[0-9]+(A|B|a|b)')]],
    lineType: ['', Validators.required] ,
    description: [''],
    color: ['blue']
    });
}
onChange() {
  this.line.LineType = this.profileForm.controls.lineType.value;
}

onChangeView(flag: string) {



   if (this.line.LineId !== '0') {
    this.flag = flag + 'Line';
  } else {
    this.flag = flag;
  }
}

getLine(lineId: string) {
  if (this.flag === 'edit' || this.flag === 'delete') {
    this.flag = this.flag + 'Line';
  }
  this.lineService.getLine(lineId).subscribe(line => {
        this.line = line;
        this.newLine = new LineSt(line.LineId, line.LineType, line.Description, line.Color);
        this.profileForm.patchValue({
          lineNumber: line.LineId,
          description: line.Description,
          lineType: line.LineType,
          color: line.Color
        });
        console.log(line);
    });
}

someFunction(lineId: string) {
  this.getLine(lineId);
}

ok() {
  this.modalService.dismissAll();
}

}
