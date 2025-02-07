import { Component } from '@angular/core';
import { FormControl, FormGroup} from '@angular/forms';
import { job_signal, CJob } from '../../Services/job';

@Component({
  selector: 'app-home',
  standalone: false,

  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})

export class HomeComponent {

  homeForm=new FormGroup(
  {
    description: new FormControl('description'),
  })

  Job$ = job_signal;

  value = input(0);
  
  //--- onTest ----------------------
  onTest()
  {
    console.trace("name: "+this.Job$().name);
    console.trace("description: "+this.Job$().description);
    console.trace("speed: "+this.Job$().speed);

  }
}
