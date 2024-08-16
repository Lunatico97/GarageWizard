// Making a custom pipe for capitalizing headers of table that is dynamically rendered from JSON
// Author: Diwas Adhikari

import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
    name : "cap"
})
export class CapitalizePipe implements PipeTransform
{
    transform(value: string, ...args: any[]) {
        let first = value.charAt(0).toUpperCase();
        return first + value.substring(1); 
    }
}