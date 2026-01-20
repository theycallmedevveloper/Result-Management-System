import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StudentService } from '../services/student.service';

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule, FormsModule], // 🔥 THIS FIXES ngFor + ngModel
  templateUrl: './students.component.html'
})
export class StudentsComponent implements OnInit {

  students: any[] = [];

  newStudent = {
    studentId: 0,
    fullName: '',
    rollNo: '',
    className: ''
  };

  constructor(private studentService: StudentService) {}

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents() {
    this.studentService.getStudents().subscribe(res => {
      this.students = res;
    });
  }

  saveStudent() {
    this.studentService.saveStudent(this.newStudent).subscribe(() => {
      this.resetForm();
    });
  }

  editStudent(student: any) {
    this.newStudent = { ...student };
  }

  deleteStudent(id: number) {
    this.studentService.deleteStudent(id).subscribe(() => {
      this.loadStudents();
    });
  }

  resetForm() {
    this.newStudent = {
      studentId: 0,
      fullName: '',
      rollNo: '',
      className: ''
    };
    this.loadStudents();
  }
}
