import { Component, OnInit, NgZone } from '@angular/core';
import { CurrentLocationService } from '../current-location.service';
import { map } from 'rxjs/operators';
import { LineService } from '../line.service';
import { WebSocketLine, CurrentLocation } from '../Model';
import { strictEqual } from 'assert';
import { stringify } from '@angular/compiler/src/util';

@Component({
  selector: 'app-current-location',
  templateUrl: './current-location.component.html',
  styleUrls: ['./current-location.component.scss'],
  styles: ['agm-map {height: 500px; width: 100%;}']
})
export class CurrentLocationComponent implements OnInit {
  lineIcons = new Map<string, any>();
  linesServices = new Map<string, WebSocketLine>();
  linesId: string[];
  iconUrl = { url: 'assets/busicon.png', scaledSize: {width: 30, height: 30}};
  constructor(private ngZone: NgZone,  private lineService: LineService) {
    this.lineIcons.set('4A',  {url: 'https://i.imgur.com/Yc7qcOB.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('4B',  {url: 'https://i.imgur.com/Yc7qcOB.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('12A', {url: 'https://i.imgur.com/a3Khpw4.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('12B', {url: 'https://i.imgur.com/a3Khpw4.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('13A', {url: 'https://i.imgur.com/iw2u5kd.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('13B', {url: 'https://i.imgur.com/iw2u5kd.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('77A', {url: 'https://i.imgur.com/KfAaW2T.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('77B', {url: 'https://i.imgur.com/KfAaW2T.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('4AA',  {url: 'https://i.imgur.com/Yc7qcOB.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('4BB',  {url: 'https://i.imgur.com/Yc7qcOB.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('12AA', {url: 'https://i.imgur.com/a3Khpw4.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('12BB', {url: 'https://i.imgur.com/a3Khpw4.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('13AA', {url: 'https://i.imgur.com/iw2u5kd.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('13BB', {url: 'https://i.imgur.com/iw2u5kd.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('77AA', {url: 'https://i.imgur.com/KfAaW2T.png', scaledSize: {width: 30, height: 30}});
    this.lineIcons.set('77BB', {url: 'https://i.imgur.com/KfAaW2T.png', scaledSize: {width: 30, height: 30}});



    this.linesServices = new Map<string, WebSocketLine>();
    this.lineService.getLines().subscribe(l =>  {
      this.linesId = l;
      l.forEach(x => {
        let temp = new WebSocketLine();
        temp.isConnected = false;
        temp.lineId = x;
        temp.notification = [];

        this.linesServices.set(x, temp);

        temp = new WebSocketLine();
        temp.isConnected = false;
        temp.lineId = x + x.substr(x.length - 1, 1);
        temp.notification = [];

        this.linesServices.set(temp.lineId, temp);

      });
      this.checkConnection();
      this.subscribeForTime();

});

   }

  ngOnInit() {
  }

  getLineIds(): Array<string> {
    return Array.from( this.linesServices.keys() );
  }

  someFunction(id: string, event) {
    if (event.target.checked) {
      this.startTimer(id);
  } else {
    this.stopTimer(id);
  }
  }

  private checkConnection() {

    this.linesServices.forEach((value: WebSocketLine, key: string) => {
      value.service.startConnection().subscribe(e => {value.isConnected = e; });
  });

  }

  subscribeForTime() {
    this.linesServices.forEach((value: WebSocketLine, key: string) => {
    this.linesServices.get(value.lineId).service.registerForTimerEvents().subscribe(e => {this.onTimeEvent(value.lineId, e); });
  });


  }

  public onTimeEvent(lineId: string, time: string) {
    this.ngZone.run(() => {
      const temp =  time.split('_');
      // console.log(time);
      this.linesServices.get(temp[0]).lan = temp[1];
      this.linesServices.get(temp[0]).lng = temp[2];
      console.log(time);
    });
  }



  public startTimer(lineId: string) {
    this.linesServices.get(lineId).service.StartTimer(lineId);
  }

  public stopTimer(lineId: string) {
    this.linesServices.get(lineId).service.StopTimer(lineId);
    this.linesServices.get(lineId).lan = '';
    this.linesServices.get(lineId).lng = '';
    this.linesServices.get(lineId + lineId.substr(lineId.length - 1, 1)).lan = '';
    this.linesServices.get(lineId + lineId.substr(lineId.length - 1, 1)).lng = '';
  }

   getCurrentLocation(): Array<WebSocketLine> {
      return Array.from( this.linesServices.values() );
   }
}

