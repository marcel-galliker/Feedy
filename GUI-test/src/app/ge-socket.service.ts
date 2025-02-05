
// https://medium.com/@sehban.alam/integrating-socket-io-with-angular-real-time-awesomeness-made-easy-039dabf97c7a

import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { increment, decrement, reset } from './counter.actions';

@Injectable(
  {providedIn: 'root'}
)

export class GESocketService
{

  private socket = new WebSocket('ws://localhost:3000');
  // socket;// = new WebSocket('ws://localhost:1234');

  constructor(private store: Store<{ count: number }>)
  {
  //  this.socket = new WebSocket("ws://localhost:1234");
    console.trace("GESocketService INITIALIZED");
  //  this.socket.send("***CONNECTED***");
    this.connect();
  }

  //--- connect ----------------------------
  connect(): void {

     this.socket.onopen = () => {
      console.log('WebSocket connection established.');
    };

    this.socket.onmessage = (event) => {
      console.log('Received message:', event.data);
      this.store.dispatch(increment());
    };

    this.socket.onclose = (event) => {
      console.log('WebSocket connection closed:', event);
    };

    this.socket.onerror = (error) => {
      console.error('WebSocket error:', error);
    };
  }

  //--- init ------------------
  public init()
  {
    console.trace("init");
//    action("TEST");
  }

  //--- sendMessage ------------------------
  public send(msg: string)
  {
      console.trace("send >>"+msg+"<<");
    this.socket.send(msg);
  }

  /*
  sendMessage(message: string): void {
    this.socket.send(message);
  }
    */

  //--- closeConnection ----------------------
  closeConnection(): void {
    this.socket.close();
  }


  }
