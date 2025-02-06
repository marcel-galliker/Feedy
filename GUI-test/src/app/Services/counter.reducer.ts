import { createReducer, on } from '@ngrx/store';
import { increment, decrement, reset, set } from './counter.actions';
import { state } from '@angular/animations';

export const initialState = 0;

export const counterReducer = createReducer(
  initialState,
  on(increment, (state) => state + 1),
  on(decrement, (state) => state - 1),
  on(reset, (state) => 0),
  on(set,   (state, {val} ) => val.valueOf()),
);
