import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode= false;
  constructor(private http: HttpClient) { }
  users:any;

  ngOnInit(): void {
   // this.getUsers();
  }

    registerToglle()
    {
      this.registerMode=!this.registerMode;
    }


    //temporary
    getUsers()
    {
          this.http.get('https://localhost:5001/api/Users').subscribe(
             response =>{
             this.users = response;
           },
           error  => {
             console.log(error);
           }
       )


    }
        cancelRegisterMode(event :boolean)
        {
            this.registerMode = event;
        }
}
