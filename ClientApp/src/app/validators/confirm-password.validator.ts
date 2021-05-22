import { AbstractControl, FormGroup, ValidationErrors } from '@angular/forms';
 
export function ConfirmedValidator
(controlName: string, matchingControlName: string) :ValidationErrors|null {
  return (controls: AbstractControl) => {
    const control = controls.get(controlName);
    const matchingControl = controls.get(matchingControlName);
    if (matchingControl?.errors && !matchingControl.errors.confirmedValidator) {
      return null;
    }
    if (control?.value !== matchingControl?.value) {
      matchingControl?.setErrors({ confirmedValidator: true });
      return ({ confirmedValidator: true });
    } else {
      matchingControl?.setErrors(null);
      return null;
    }

    return null;
  }
}
