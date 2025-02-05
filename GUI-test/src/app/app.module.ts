import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import {ReactiveFormsModule} from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NameEditorComponent } from './name-editor/name-editor.component';
import { GESocketService } from './ge-socket.service';
import { SocketIoModule, SocketIoConfig } from 'ngx-socket-io';
import { StoreModule } from '@ngrx/store';
// import { WebSocketService } from './ge-socket.service';

@NgModule({
  declarations: [
    AppComponent,
    NameEditorComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    StoreModule.forRoot({}, {}),
  //  SocketIoModule.forRoot(config)
  ],
  providers: [
    GESocketService,
 //   WebSocketService,
    provideClientHydration(withEventReplay()),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
