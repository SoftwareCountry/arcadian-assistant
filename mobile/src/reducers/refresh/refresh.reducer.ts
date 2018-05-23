import { Reducer } from 'redux';
import { combineEpics } from 'redux-observable';
import { refreshEpic$ } from './refresh.epics';

export const refreshEpics = combineEpics(refreshEpic$);
