import { Component } from '@angular/core';
import { NameEditorComponent} from './name-editor/name-editor.component';
import { GESocketService} from './ge-socket.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css',
//  providers: [GESocketService]
})
export class AppComponent {
  title = 'FORMS TEST';
  constructor()
  {
  }


}
