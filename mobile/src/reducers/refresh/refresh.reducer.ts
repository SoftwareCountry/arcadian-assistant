import { combineEpics } from 'redux-observable';
import { refreshEpic$, refreshUserProfileData$ } from './refresh.epics';

export const refreshEpics = combineEpics(refreshEpic$, refreshUserProfileData$);
