import { combineEpics } from 'redux-observable';
import {
    loadUserEmployeePermissionsEpic$,
    loadUserEpic$,
    loadUserFinishedEpic$,
    loadUserPreferencesEpic$,
    updateUserPreferencesEpic$
} from './user.epics';

export const userEpics = combineEpics(
    loadUserEpic$ as any,
    loadUserFinishedEpic$ as any,
    loadUserEmployeePermissionsEpic$ as any,
    loadUserPreferencesEpic$ as any,
    updateUserPreferencesEpic$ as any,
);
