import { Component, inject } from '@angular/core';
import {FormControl, FormGroup} from '@angular/forms';
import { GESocketService } from './../ge-socket.service';
import { Store } from '@ngrx/store';
import { increment, decrement, reset } from '../counter.actions';

@Component({
  selector: 'app-name-editor',
  standalone: false,

  templateUrl: './name-editor.component.html',
  styleUrl: './name-editor.component.css',
})

export class NameEditorComponent {

  private count=0;

  constructor(private socket: GESocketService, private store: Store<{ count: number }>)
  {
//    socket.init(store);
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
