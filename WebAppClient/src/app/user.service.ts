import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';

import { Observable, of, Subject, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { User, LoginUser, UserStatus } from './Model';
import { Router } from '@angular/router';



const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json'  })
};




@Injectable({ providedIn: 'root' })
export class UserService {
  private UserRegisterUrl = 'http://localhost:52295/api/user/register';
  private UserChangeUrl = 'http://localhost:52295/api/user/changeinfo';
  private UserStatusChangeUrl = 'http://localhost:52295/api/user/changeuserstatus';
  private UserGetUserrUrl = 'http://localhost:52295/api/user/GetInfo';
  private UserGetUsersrUrl = 'http://localhost:52295/api/user/getusers';
  private UserLoginUrl = 'http://localhost:52295/oauth/token';

  // ZA LOGIN
  // zavisno od promene username-a, ja ga predstavljam kao Observable na servisu, i onda se subscribujem na taj stream u komponenti
  username: Observable<string>;
  private userSubject: Subject<string>;

  // ZA PRIKAZ INFO OPCIJE (na osnovu role)
  role: Observable<string>;
  private roleSubject: Subject<string>;

  // ZA USER INFO
  editUser: Observable<User>;
  private editUserSubject: Subject<User>;

  constructor(private http: HttpClient, private router: Router) {
    this.userSubject = new Subject<string>();
    this.username = this.userSubject.asObservable();

    this.roleSubject = new Subject<string>();
    this.role = this.roleSubject.asObservable();

    this.editUserSubject = new Subject<User>();
    this.editUser = this.editUserSubject.asObservable();
  }


addUser(user: User): Observable<User> {
   return this.http.post<User>(this.UserRegisterUrl, user, httpOptions).pipe(catchError(this.errorHandler));
}

changeUser(user: User): Observable<User> {
  return this.http.post<User>(this.UserChangeUrl, user).pipe(catchError(this.errorHandler));
}

changeUserStatus(user: UserStatus): Observable<UserStatus> {
  return this.http.post<UserStatus>(this.UserStatusChangeUrl, user).pipe(catchError(this.errorHandler));
}

getusers(userStatus: number): Observable<UserStatus[]> {
  const url = `${this.UserGetUsersrUrl}?userStatus=${userStatus}`;
  return this.http.get<UserStatus[]>(url).pipe(
    catchError(this.handleError<UserStatus[]>(`getusers id=${userStatus}`))
  );
}

errorHandler(error: HttpErrorResponse) {
  if (error.error instanceof ErrorEvent) {
    console.error('An error occurred: ', error.error.message);
  } else {
    console.error(
      `Backend returned code ${error.status}` + `body was: ${error.error.error_description}`
    );
  }

  return throwError('Something bad happend, please try again later...');
}

getUser(username: string): Observable<User> {
  const url = `${this.UserGetUserrUrl}?username=${username}`;
  const jwt = localStorage.getItem('jwt');
  console.log(jwt);

  return this.http.get<User>(url).pipe(
    catchError(this.handleError<User>(`getUser username=${username}`))
  );

}

logIn(user: LoginUser, callback: any) {
  const data = `username=${user.Username}&password=${user.Password}&grant_type=password`;
// tslint:disable-next-line: no-shadowed-variable
  const httpOptions = {
      headers: {
          'Content-type': 'application/x-www-form-urlencoded'
      }
  };
  const retVal = 'Username or password is incorrect';
  this.http.post<any>(this.UserLoginUrl, data, httpOptions)
// tslint:disable-next-line: no-shadowed-variable
  .subscribe (data => {

    const jwt = data.access_token;

    const jwtData = jwt.split('.')[1];
    const decodedJwtJsonData = window.atob(jwtData);
    const decodedJwtData = JSON.parse(decodedJwtJsonData);

    const role = decodedJwtData.role;

    if (localStorage.length === 0) {
      localStorage.setItem('jwt', jwt);
      localStorage.setItem('role', role);
      localStorage.setItem('username',  decodedJwtData.unique_name);
    }

    this.username = decodedJwtData.unique_name;
    this.userSubject.next(decodedJwtData.unique_name);
    this.role = decodedJwtData.role;
    this.roleSubject.next(decodedJwtData.role);

    callback();
  },
  error => {
    callback();
  });
}

logout() {
  localStorage.clear();
  this.router.navigate(['/home']);
}

private handleError<T>(operation = 'operation', result?: T) {
  return (error: any): Observable<T> => {
    return of(result as T);
  };
}
}
