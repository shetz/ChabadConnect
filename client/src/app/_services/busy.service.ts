import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { unzipSync } from 'zlib';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busyRequestCount = 0;
  constructor(private spinnerService: NgxSpinnerService) {}

  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'ball-scale-multiple',
      bdColor: 'rgba(51,51,51,0.8)',
      size: 'medium',
      color: '#fff',
    });
  }

  idle() {
    this.busyRequestCount--;
    if(this.busyRequestCount <= 0){
       this.busyRequestCount=0;
       this.spinnerService.hide();
    }

  }
}
