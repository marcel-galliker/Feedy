import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NameEditorComponent } from './Components/name-editor/name-editor.component';//'./components/name-editor/name-editor.component';
import { GESocketService } from './Services/ge-socket.service';
import { StoreModule } from '@ngrx/store';
import { counterReducer } from './Services/counter.reducer';
import { MyCounterComponent } from './Components/my-counter/my-counter.component';
import { MenuComponent } from './Components/menu/menu.component';
import { HomeComponent } from './Components/home/home.component';
import { JobComponent } from './Components/job/job.component';

@NgModule({
  declarations: [
    AppComponent,
    NameEditorComponent,
    MyCounterComponent,
    MenuComponent,
    HomeComponent,
    JobComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
   // StoreModule.forRoot({}, {}),
    StoreModule.forRoot({ count: counterReducer })
  ],
  providers: [
    GESocketService,
    provideClientHydration(withEventReplay()),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
