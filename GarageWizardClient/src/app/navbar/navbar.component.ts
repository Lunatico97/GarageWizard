import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  @Input() navlinks : string[] = [];
  @Input() navTitles : string[] = [];
  @Input() navImg: string[] = [];
  activeLink: string = "";

  ngOnInit(): void {
    this.activeLink = this.navlinks[0];
  }

}
