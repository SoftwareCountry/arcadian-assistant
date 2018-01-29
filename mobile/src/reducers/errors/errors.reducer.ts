import { combineEpics } from 'redux-observable';
import { loadFailedErrorEpic$ } from '../errors/errors.epics';

export const errorsEpics = combineEpics(loadFailedErrorEpic$);
