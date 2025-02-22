import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { UrlService, UrlDetailsDto } from '../../services/url.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-url-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ],
  templateUrl: './url-details.component.html',
  styleUrls: ['./url-details.component.scss']
})
export class UrlDetailsComponent implements OnInit {
  details: UrlDetailsDto | null = null;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private urlService: UrlService,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    if (!this.auth.isAuthenticated()) {
      this.errorMessage = 'You must be logged in to see details.';
      return;
    }
    this.loadDetails();
  }

  loadDetails() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      this.errorMessage = 'Invalid URL id.';
      return;
    }
    const id = Number(idParam);

    this.urlService.getUrlDetails(id).subscribe({
      next: (res) => {
        this.details = res;
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to load details.';
      }
    });
  }

  goBack() {
    this.router.navigate(['/urls']);
  }
}
