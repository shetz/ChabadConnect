import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/memebers.service';

@Component({
  selector: 'app-memeber-card',
  templateUrl: './memeber-card.component.html',
  styleUrls: ['./memeber-card.component.css'],
  //encapsulation:ViewEncapsulation.None //if you want global styles
})
export class MemeberCardComponent implements OnInit {
  @Input() member: Member;

  constructor(
    private membersService: MembersService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  addLike(member: Member){
    this.membersService
      .addLike(member.username)
      .subscribe(() =>{
        this.toastr.success('You have Liked ' + member.username);
      }

      );
  }
}
