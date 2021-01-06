import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Member } from '../models/member';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';

//primitive way of sending token
// const httpOptions={
//   headers: new HttpHeaders({
//     Authorization:'Bearer '+ JSON.parse(localStorage.getItem('user'))?.token
//   }

//   )
// }
@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  members :Member[]=[];

  constructor( private http: HttpClient) { }
  getMembers()
  {
    if(this.members.length>0) return of(this.members);
    // if no members in app get from server
     return this.http.get<Member[]>(this.baseUrl+ 'users').pipe(
       map(members =>{
         this.members= members;
         return members;
       })
     );
  }

  getMember(username: string)
  {
    const member = this.members.find(x => x.username === username);
    if (member !== undefined)
    return of (member);

    return this.http.get<Member>(this.baseUrl+ 'users/'+username);
  }

  updateMember(member: Member)
  {
    return this.http.put<Member>(this.baseUrl+ 'users/', member).pipe(
      map(()=>{
        const index = this.members.indexOf(member);
        this.members[index]= member;
      })
    )
  }

}
