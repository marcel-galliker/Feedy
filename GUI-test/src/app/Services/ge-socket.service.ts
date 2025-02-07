import { props } from '@ngrx/store';

// https://medium.com/@sehban.alam/integrating-socket-io-with-angular-real-time-awesomeness-made-easy-039dabf97c7a

import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { increment, decrement, reset } from './counter.actions';
import { status_signal } from '../Services/status';

@Injectable(
  {providedIn: 'root'}
)

export class GESocketService
{
  private socket = new WebSocket('ws://localhost:3000');
  private status$ = status_signal;
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
      console.log('Received message: >>'+event.data+"<<");
      try {
        var obj = JSON.parse(event.data);
//        console.trace("props>>"+obj.hasOwnProperty('name')+"<<");
        if (obj.msgid=="STATUS") this.status$.set(obj);
      }
      catch(e)
      {
      }
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
