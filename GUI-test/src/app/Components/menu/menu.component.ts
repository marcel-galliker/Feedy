import { Component, effect } from '@angular/core';
import { menu_selection, CMenuSignal } from '../../Services/signals';

@Component({
  selector: 'app-menu',
  standalone: false,

  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})

export class MenuComponent {

  private menu_selection$ = menu_selection;
  private selectedCompoment : string = "";

  //--- constructor ---------------------------------------
  constructor()
  {
     effect(() => {this.onMennu(this.menu_selection$()); });
  }

  //--- OnMenu -----------------------------------
  onMennu(menu: CMenuSignal)
  {
    var element = document.getElementById(this.selectedCompoment) as HTMLElement;
    if (element!=null) {element.style.color= 'White'; element.style.fill='White';}
    this.selectedCompoment="BTN_"+menu.menu;
    element = document.getElementById(this.selectedCompoment) as HTMLElement;
    if (element!=null) {element.style.color='orange'; element.style.fill='orange'};
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
