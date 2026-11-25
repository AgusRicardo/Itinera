import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavegacionService } from './core/services/navegacion.service';
import { ConfirmacionComponent } from './shared/components/confirmacion/confirmacion.component';
import { ToastrComponent } from './shared/components/toastr/toastr.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ConfirmacionComponent, ToastrComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly navegacion = inject(NavegacionService);

  protected readonly title = signal('Itinera.Web');
}
