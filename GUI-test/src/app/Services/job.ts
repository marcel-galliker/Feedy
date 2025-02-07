import { signal } from "@angular/core";

export class CJob
{
  msgid: string = "Job";
  name: string="name";
  description: string="Description";
  speed: number=100;
  count: number=1;
// feeder
// cam
// tray
// gripper
// pickPlace
};

export const job_signal = signal<CJob>(new CJob());

