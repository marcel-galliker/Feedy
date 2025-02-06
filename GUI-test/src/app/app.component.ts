import { Component } from '@angular/core';
import { effect } from "@angular/core";
import { menu_selection} from '../app/Services/signals';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css',
//  providers: [GESocketService]
})
export class AppComponent {
  title = 'FORMS TEST';
  private menu_selection$ = menu_selection;
  private selectedCompoment : string = "";

  constructor()
  {
    console.trace("start");

    effect(() => {
      const selected = this.menu_selection$();
      var element = document.getElementById(this.selectedCompoment) as HTMLElement;
      if (element!=null) element.style.display= 'none';
      this.selectedCompoment=selected.menu;
      element = document.getElementById(this.selectedCompoment) as HTMLElement;
      if (element!=null) element.style.display= 'block';
    });
  }

}
