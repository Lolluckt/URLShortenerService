import { Routes } from '@angular/router';
import { UrlListComponent } from './url/url-list/url-list.component';
import { UrlDetailsComponent } from './url/url-details/url-details.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { AboutComponent } from './about/about.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'urls', component: UrlListComponent },
  { path: 'urls/:id', component: UrlDetailsComponent },
  { path: 'about', component: AboutComponent },
  { path: '', redirectTo: 'login', pathMatch: 'full' }
];
