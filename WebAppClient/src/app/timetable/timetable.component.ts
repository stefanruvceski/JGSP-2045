import { Component, OnInit, Input } from '@angular/core';
import { Line } from '../Model';
import { TimetableService } from '../timetable.service';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
  selector: 'app-timetable',
  templateUrl: './timetable.component.html',
  styleUrls: ['./timetable.component.scss']
})

export class TimetableComponent implements OnInit {
  @Input() lines: Line[];
  @Input() timetable: string[];
  toogled: boolean;
  hid: boolean;
  lineId: string;
  lineDesc: string;
  day: string;


  constructor(private timetableService: TimetableService) { }

  ngOnInit() {
    this.toogled = false;
    this.hid = false;
    this.getLines('Urban');
    this.day = 'Weekday';

  }

  getLines(type: string) {
    this.hid = false;
    this.lineId = null;
    this.timetableService.getLines(type).subscribe(lines => this.lines = lines);
  }

  showLineDetails(id: string) {
    this.timetableService.getTimetable(id, this.day).subscribe(timetable => this.timetable = timetable);

    this.lines.forEach(item => {
      if (id === item.Lineid) {
        this.lineId = id;
        this.lineDesc = item.Description.toString();
      }
   });
    this.hid = true;
  }
  togle(flag: boolean) {

    this.toogled = !this.toogled;
    if (this.toogled) {
      this.getLines('Suburban');
    } else {
      this.getLines('Urban');
    }

  }

  onSelectionChange(day: string) {
    this.day = day;
    if (this.lineId) {
      this.showLineDetails(this.lineId);
    }
  }

}
