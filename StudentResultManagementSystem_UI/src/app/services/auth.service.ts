import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'https://localhost:7240/api/auth';

  constructor(private http: HttpClient) {}

  login(username: string, password: string) {
    return this.http.post(
      `${this.apiUrl}/login`,
      { username, password },
      { withCredentials: true }
    );  
  }

  logout() {
    return this.http.post(
      `${this.apiUrl}/logout`,
      {},
      { withCredentials: true }
    );
  }

  profile() {
    return this.http.get<any>(
      `${this.apiUrl}/profile`,
      { withCredentials: true }
    );
  }
}
