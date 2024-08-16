import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'round'
})
export class RoundPipe implements PipeTransform {
  transform(value: number, decimalPlaces: number = 0): number {
    if (isNaN(value) || isNaN(decimalPlaces)) return value;

    const factor = Math.pow(10, decimalPlaces);
    return Math.round(value * factor) / factor;
  }
}