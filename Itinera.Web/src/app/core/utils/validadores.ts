import { AbstractControl, ValidationErrors } from '@angular/forms';

export function textoRequerido(control: AbstractControl<string>): ValidationErrors | null {
  return control.value.trim() ? null : { required: true };
}

export function telefonoValido(control: AbstractControl<string>): ValidationErrors | null {
  return !control.value || /^(?=(?:[^0-9]*[0-9]){6})[0-9 ()+./-]+$/.test(control.value)
    ? null
    : { telefono: true };
}

export function cuitValido(control: AbstractControl<string>): ValidationErrors | null {
  return !control.value.trim() || /^[0-9]{2}-?[0-9]{8}-?[0-9]$/.test(control.value.trim())
    ? null
    : { cuit: true };
}
