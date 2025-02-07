// https://blog.angular-university.io/angular-signals/


import { signal } from "@angular/core";

export class CMenuSignal
{
  menu: string = "";
};

export const menu_selection = signal<CMenuSignal>({menu: "Home"});
