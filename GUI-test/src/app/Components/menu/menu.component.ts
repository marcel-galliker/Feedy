import { Component } from '@angular/core';
import { effect } from "@angular/core";
import { menu_selection} from '../../Services/signals';

@Component({
  selector: 'app-menu',
  standalone: false,

  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})

export class MenuComponent {

  menu_selection$ = menu_selection;
  private selectedCompoment : string = "";

  //--- constructor ---------------------------------------
  constructor()
  {
     effect(() => {
          const selected = this.menu_selection$();
          var element = document.getElementById(this.selectedCompoment) as HTMLElement;
          if (element!=null) {element.style.color= 'White'; element.style.fill='White';}
          this.selectedCompoment="BTN_"+selected.menu;
          element = document.getElementById(this.selectedCompoment) as HTMLElement;
          if (element!=null) {element.style.color='Red'; element.style.fill='Red'};
        });
  }

  //--- onHome ------------------------------------
  onHome()
  {
    this.menu_selection$.set({menu:"Home"});
  }

  //--- onJob ------------------------------------
  onJob()
  {
    this.menu_selection$.set({menu:"Job"});
  }

  //--- onPickPlace ------------------------------------
  onPickPlace()
  {
    this.menu_selection$.set({menu:"PickPlace"});
  }

  //--- onEvents ------------------------------------
  onEvents()
  {
    this.menu_selection$.set({menu:"Events"});
  }

  //--- onLog ------------------------------------
  onLog()
  {
    this.menu_selection$.set({menu:"Log"});

  }

  //--- onConfig ------------------------------------
  onConfig()
  {
    this.menu_selection$.set({menu:"Config"});
  }

  //--- onSettings ------------------------------------
  onSettings()
  {
    this.menu_selection$.set({menu:"Settings"});
  }


  //--- onUser ------------------------------------
  onUser()
  {
    this.menu_selection$.set({menu:"User"});
  }
}
