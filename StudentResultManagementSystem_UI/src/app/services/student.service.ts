import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Student } from '../models/student.model';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private apiUrl = 'https://localhost:7240/api/students';

  constructor(private http: HttpClient) {}

  getStudents() {
    return this.http.get<Student[]>(this.apiUrl, { withCredentials: true });
  }

  addStudent(student: Student) {
    return this.http.post(this.apiUrl, student, { withCredentials: true });
  }

  deleteStudent(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`, { withCredentials: true });
  }
}
