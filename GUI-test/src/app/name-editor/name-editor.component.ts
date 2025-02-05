import { Component, inject } from '@angular/core';
import {FormControl, FormGroup} from '@angular/forms';
import { GESocketService } from './../ge-socket.service';
// import { Socket } from 'ngx-socket-io';
import { map } from 'rxjs/operators';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
//import { WebSocketService } from './../ge-socket.service';

import { io, Socket } from 'socket.io-client';

@Component({
  selector: 'app-name-editor',
  standalone: false,

  templateUrl: './name-editor.component.html',
  styleUrl: './name-editor.component.css',
//  providers: [GESocketService]
})

export class NameEditorComponent {

  private count=0;

  constructor(private socket: GESocketService)
  {
    socket.init();
  }

  userForm=new FormGroup(
  {
    name: new FormControl('name'),
    address: new FormControl('address'),
  })

  onSubmit() {
    // TODO: Use EventEmitter with form value
    console.warn(this.userForm.value);
  }

  updateName()
  {
    this.userForm.patchValue({name:'Nancy'});
    this.count++;
    this.socket.send("msg "+this.count+":12345678901234567890"
      +"12345678901234567890"
      +"12345678901234567890"
      +"12345678901234567890"
      +"12345678901234567890"
      +"12345678901234567890"
      +"12345678"
    );
  }

}
