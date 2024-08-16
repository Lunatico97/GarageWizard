// Making a custom pipe for range iterations in for loops aka ngFor directives
// Author: Diwas Adhikari

import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
    name : "range"
})
export class RangePipe implements PipeTransform
{
    transform(value: number, ...args: any[]) {
        var rangeList = new Array(value).map((number, index) => index);
        return rangeList;
    }
}