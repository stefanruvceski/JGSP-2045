import { Component, OnInit, Input } from '@angular/core';
import { TimetableService } from '../timetable.service';
import { Line, TimeTable } from '../Model';
import { FormBuilder, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-admin-timetable',
  templateUrl: './admin-timetable.component.html',
  styleUrls: ['./admin-timetable.component.scss']
})


export class AdminTimetableComponent implements OnInit {
  @Input() lines: Line[];
  @Input() timetable: string[];
  toogled: boolean;
  hid: boolean;
  tt: TimeTable;
  feedback: string;
  addchange = 'Edit';
  lineId: string;
  lineDesc: string;
  day: string;
  profileForm = this.fb.group({
    timetableF: ['', Validators.required],

    });

  constructor(private fb: FormBuilder, private timetableService: TimetableService, private modalService: NgbModal) { }

  ngOnInit() {
    this.toogled = false;
    this.hid = false;
    this.getLines('Urban');
    this.day = 'Weekday';

  }

  getLines(type: string) {
    this.hid = false;
    this.lineId = null;
    this.timetableService.getLines(type).subscribe(lines => {
      this.lines = lines;


    });
  }

  showLineDetails(id: string) {
    this.timetableService.getTimetable(id, this.day).subscribe(timetable => {
      this.timetable = timetable;
      if (!timetable) {
        this.profileForm.patchValue({
          timetableF: '',
        });
      } else {
        this.profileForm.patchValue({
          timetableF: timetable.toString().split(',').join('\n'),
        });
      }

    });

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
    if (day === 'edit' ) {
      this.addchange = 'Edit';
      this.profileForm.patchValue({
        timetableF: this.timetable.toString().split(',').join('\n'),
      });
    } else if (day === 'add') {
      this.addchange = 'Add';
      this.profileForm.patchValue({
        timetableF: '',
      });
    } else {
      this.day = day;
      if (this.lineId) {
        this.showLineDetails(this.lineId);
      }
    }
  }
  deleteTimetable() {
    this.timetableService.deleteTimetable(this.lineId, this.day).subscribe(data => window.location.reload());
  }
  openWindowCustomClass(content) {
    this.modalService.open(content, { windowClass: 'dark-modal' });
  }
  onSubmit(content) {
// tslint:disable-next-line: max-line-length
    this.tt = new TimeTable( this.lineId, this.day, this.profileForm.controls.timetableF.value.split('\n').join('|'));
    console.log(this.tt);

    this.timetableService.setTimetable(this.tt).subscribe(data => {
      this.feedback = 'Successfuly ' + this.addchange + ' timetable';
      this.openWindowCustomClass(content);
    }, error => {
      this.feedback = 'Unsuccessfuly ' + this.addchange + ' timetable';
      this.openWindowCustomClass(content);
    });
  }

  ok() {
    this.modalService.dismissAll();
  }

}
