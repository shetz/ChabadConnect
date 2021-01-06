import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Member } from 'src/app/models/member';

@Component({
  selector: 'app-memeber-card',
  templateUrl: './memeber-card.component.html',
  styleUrls: ['./memeber-card.component.css']
  //encapsulation:ViewEncapsulation.None //if you want global styles
})
export class MemeberCardComponent implements OnInit {
@Input() member: Member;

  constructor() { }

  ngOnInit(): void {
  }

}
