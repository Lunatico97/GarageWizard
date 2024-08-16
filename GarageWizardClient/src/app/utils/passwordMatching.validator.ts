// Custom validator for password matching

import { FormGroup, ValidationErrors } from "@angular/forms";

export function PasswordMatchingValidator(form: FormGroup | any): ValidationErrors | null
{
    if(form.get('Password')?.value === form.get('ConfirmPassword')?.value)
    {
        return null;
    }
    return {validate: false, message: 'Both passwords do not match !'};
}