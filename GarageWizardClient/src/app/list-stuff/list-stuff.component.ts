import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
  selector: 'list-stuff',
  styleUrls: ['list-stuff.component.scss'],
  templateUrl: 'list-stuff.component.html',
})
export class ListStuffComponent {
  displayedColumns!: string[];
  displayedNewColumns!: string[];
  @Input() dataSource! : any[];
  @Input() enableActions: boolean = false;
  @Input() viewDisabled: boolean = false;
  @Input() key!: string;

  constructor(){}

  ngOnInit() : void
  {
    this.displayedColumns = Object.keys(this.dataSource[0] ?? []);
    if(this.enableActions)
      this.displayedNewColumns = this.displayedColumns.concat(['actions']);
    else 
      this.displayedNewColumns = this.displayedColumns;
  }

  @Input() onDeleteCallback!: (key: string) => void;
  @Input() onViewCallback!: (key: string) => void;
}
