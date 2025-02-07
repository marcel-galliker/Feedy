import { signal } from "@angular/core";

export class CStatus
{
  msgid: string = "";
  count: number = 0;
  name: string="";
  speed: number=1.234;
};

export const status_signal = signal<CStatus>(new CStatus());

