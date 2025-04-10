import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface UrlDto {
  id: number;
  originalUrl: string;
  shortUrl: string;
  createdDate: string;
  createdByEmail: string;
}

export interface UrlDetailsDto {
  id: number;
  originalUrl: string;
  shortUrl: string;
  createdByUserId: number;
  createdByEmail: string;
  createdDate: string;
  updatedDate?: string;
  isDeleted: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  private http = inject(HttpClient);
  private baseUrl = 'https://urlservice-backend.onrender.com/api/url';

  getAllUrls() {
    return this.http.get<UrlDto[]>(`${this.baseUrl}/all`);
  }

  createUrl(originalUrl: string) {
    return this.http.post<UrlDto>(`${this.baseUrl}/create`, { originalUrl });
  }

  deleteUrl(id: number) {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  getUrlDetails(id: number) {
    return this.http.get<UrlDetailsDto>(`${this.baseUrl}/${id}`);
  }
}
