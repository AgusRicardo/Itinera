import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastrComponent } from './shared/components/toastr/toastr.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToastrComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Itinera.Web');
}
