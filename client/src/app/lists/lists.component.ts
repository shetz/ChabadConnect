import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagainataion';
import { MembersService } from '../_services/memebers.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = 'liked';
  PageNumber = 1;
  PageSize = 5;
  pagination: Pagination;;
  constructor(private membersService: MembersService) {}

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    //console.log('page no is ' + this.PageNumber.toString());
    this.membersService
      .getLikes(this.predicate, this.PageNumber, this.PageSize)
      .subscribe((response) => {
        this.members = response.result;
        this.pagination = response.pagination;
      });
  }

  pageChanged(event: any)
  {
    this.PageNumber = event.page;
    this.loadLikes();
  }
}
