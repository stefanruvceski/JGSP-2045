import { CurrentLocationService } from './current-location.service';

export class WebSocketLine {
  lineId: string;
  service: CurrentLocationService;
  isConnected: boolean;
  notification: string[];
  lan: string;
  lng: string;

  constructor() {
    this.lineId = '0';
    this.service = new CurrentLocationService();
    this.isConnected = false;
    this.notification = [];
    this.lan = '0';
    this.lng = '0';
  }
}


export class PriceListAdmin {
  IssueDate: string;
  ExpireDate: string;
  TimeTicketPrice: string;
  DailyTicketPrice: string;
  MonthlyTicketPrice: string;
  AnnualTicketPrice: string;

  constructor(private issueDate: string, private expireDate: string, private timeTicketPrice: string,
              private dailyTicketPrice: string, private monthlyTicketPrice: string, private annualTicketPrice: string) {
    this.IssueDate = issueDate;
    this.ExpireDate = expireDate;
    this.TimeTicketPrice = timeTicketPrice;
    this.DailyTicketPrice = dailyTicketPrice;
    this.MonthlyTicketPrice = monthlyTicketPrice;
    this.AnnualTicketPrice = annualTicketPrice;
  }
}

export class TimeTable {
  LineId: string;
  Day: string;
  Schedule: string;

  constructor(private lineid: string, private day: string, private schedule: string) {
    this.LineId = lineid;
    this.Day = day;
    this.Schedule = schedule;

  }
}

export class LineSt {
  LineId: string;
  LineType: string;
  Description: string;
  Color: string;
  Stations: Array<Station>;

  constructor(private lineid: string, private linetype: string, private description: string, private color: string) {
    this.LineId = lineid;
    this.LineType = linetype;
    this.Description = description;
    this.Color = color;
    this.Stations = new Array<Station>();
  }
}

export class Station {
  Name: string;
  Address: string;
  XCoordinate: number;
  YCoordinate: number;
  IsStation: string;



  constructor(private name: string, private address: string, private xCoordinate: number,
              private yCoordinate: number, private isStation: string) {
    this.Name = name;
    this.Address = address;
    this.XCoordinate = xCoordinate;
    this.YCoordinate = yCoordinate;
    this.IsStation = isStation;
  }

}

export class Pricelist {
  TicketName: string;
  Price: number;
}

export class Line {
  Lineid: string;
  Description: number;
}

export class User {
  Username: string;
  FirstName: string;
  LastName: string;
  Password: string;
  ConfirmPassword: string;
  Address: string;
  AgeGroup: string;
  Document: string;
  Birthday: string;
  Email: string;
}

export class UserStatus {
  Username: string;
  FirstName: string;
  LastName: string;
  AgeGroup: string;
  Document: string;
  Birthday: string;
  Status: string;
}

export class LoginUser {
  Username: string;
  Password: string;
}

export class Ticket {
  Id: string;
  TicketTypeId: string;
  AgeGroup: string;
  Price: string;
  IssuingTime: string;
  ExpirationTime: string;
  Description: string;
}

export class TicketVerification {
  Status: string;
  Description: string;
}

export class UserTicket {
  TicketId: string;
  IssuingTime: string;
  ExpirationTime: string;
  TicketType: string;
}

export class CurrentLocation {
  lineId: string;
  color: string;
  lan: string;
  lng: string;

  constructor(){
    this.lineId = '0';
    this.color = 'black';
    this.lan = '0';
    this.lng = '0';
  }
}
