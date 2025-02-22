import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';

interface AboutDto {
  id: number;
  content: string;
  modifiedByUserId?: number;
  modifiedDate?: string; // или Date
}

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent implements OnInit {
  aboutContent = '';
  originalContent = '';
  isEditing = false;
  errorMessage = '';
  aboutId = 0;

  constructor(
    private http: HttpClient,
    public auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAbout();
  }

  loadAbout() {
    this.http.get<AboutDto>('https://localhost:7266/api/about')
      .subscribe({
        next: (res) => {
          this.aboutId = res.id;
          this.aboutContent = res.content;
          this.originalContent = res.content;
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Failed to load About.';
        }
      });
  }

  get isAdmin(): boolean {
    return this.auth.isAdmin();
  }

  startEditing() {
    this.isEditing = true;
    this.errorMessage = '';
    this.originalContent = this.aboutContent;
  }

  cancelEditing() {
    this.isEditing = false;
    this.aboutContent = this.originalContent;
  }

  saveAbout() {
    const dto = {
      id: this.aboutId,
      content: this.aboutContent
    };

    this.http.post('https://localhost:7266/api/about', dto)
      .subscribe({
        next: () => {
          this.isEditing = false;
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Failed to update About.';
        }
      });
  }

  goBack() {
    this.router.navigate(['/urls']);
  }
}
