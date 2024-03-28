import { Injectable } from '@angular/core';
import { UserForRegistrationDto } from './../../_interfaces/user/userForRegistrationDto.model';
import { RegistrationResponseDto } from './../../_interfaces/response/registrationResponseDto.model';
import { HttpClient } from '@angular/common/http';
import { EnvironmentUrlService } from './environment-url.service';
import { UserForAuthenticationDto } from './../../_interfaces/user/userForAuthenticationDto.model';
import { AuthResponseDto } from './../../_interfaces/response/authResponseDto.model';
import { Subject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Claim } from '../../_interfaces/response/Claim';


@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private authChangeSub = new Subject<boolean>()
  public authChanged = this.authChangeSub.asObservable();

  constructor(
    private http: HttpClient,
    private envUrl: EnvironmentUrlService,
    private jwtHelper: JwtHelperService,
    private router: Router
  ) { }

  public registerUser = (route: string, body: UserForRegistrationDto) => {
    return this.http.post<RegistrationResponseDto>(this.createCompleteRoute(route, this.envUrl.urlAddress), body);
  }

  private createCompleteRoute = (route: string, envAddress: string) => {
    return `${envAddress}/${route}`;
  }

  public loginUser = (route: string, body: UserForAuthenticationDto) => {
    return this.http.post<AuthResponseDto>(this.createCompleteRoute(route, this.envUrl.urlAddress), body);
  }


  public getClaims(route: string): Observable<Claim[]> {
    return this.http.get<Claim[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
  }


  public navigateToLogin = (returnUrl: string) => {
    this.router.navigate(['/authentication/login'], { queryParams: { returnUrl: returnUrl } });
  }
  public sendAuthStateChangeNotification = (isAuthenticated: boolean) => {
    this.authChangeSub.next(isAuthenticated);
  }

  public isUserAdmin = (): boolean => {
    const token = localStorage.getItem("token");
    const decodedToken = this.jwtHelper.decodeToken(token!);
    const role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']

    if (Array.isArray(role) && role.includes("Administrator")) {
      return true;
    }
    return false;
  }

  public isUserAuthenticated = (): boolean => {
    const token = localStorage.getItem("token");

    const a = this.jwtHelper.decodeToken(token!);

    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  public logout = () => {
    localStorage.removeItem("token");
    this.sendAuthStateChangeNotification(false);
    this.navigateToLogin("");

  }
}
