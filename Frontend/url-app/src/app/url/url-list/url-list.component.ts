import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UrlService, UrlDto } from '../../services/url.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-url-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './url-list.component.html',
  styleUrls: ['./url-list.component.scss']
})
export class UrlListComponent implements OnInit {
  urls: UrlDto[] = [];
  newUrl = '';
  errorMessage = '';
  savedShortUrl = '';

  constructor(
    private urlService: UrlService,
    public auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.auth.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.loadUrls();
  }

  loadUrls() {
    this.urlService.getAllUrls().subscribe({
      next: (res) => (this.urls = res),
      error: (err) => console.error(err)
    });
  }

  addUrl() {
    this.errorMessage = '';
    this.savedShortUrl = '';

    if (!this.newUrl.trim()) return;

    this.urlService.createUrl(this.newUrl).subscribe({
      next: (createdUrl) => {
        this.newUrl = '';
        this.savedShortUrl = `https://localhost:7266/${createdUrl.shortUrl}`;
        this.loadUrls();
      },
      error: (err) => {
        if (err.status === 409) {
          this.errorMessage = err.error.message || 'URL already exists.';
        } else {
          this.errorMessage = 'Error creating URL.';
        }
      }
    });
  }

  copyShortLink(input: HTMLInputElement) {
    input.select();
    input.setSelectionRange(0, 99999);
    document.execCommand('copy');
    alert('Short link copied to clipboard!');
  }

  canDelete(url: UrlDto): boolean {
    if (!this.auth.isAuthenticated()) return false;
    if (this.auth.isAdmin()) return true;
    return url.createdByEmail === this.auth.getUserEmail();
  }

  deleteUrl(id: number) {
    this.urlService.deleteUrl(id).subscribe({
      next: () => this.loadUrls(),
      error: (err) => console.error(err)
    });
  }

  goToDetails(id: number) {
    this.router.navigate(['/urls', id]);
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
