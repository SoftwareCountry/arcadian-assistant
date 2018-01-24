import { calculateDaysCounters } from './calendar.action';
import { ActionsObservable, ofType } from 'redux-observable';
import { map } from 'rxjs/operators';
import { LoadUserFinished } from '../organization/organization.action';

export const loadEmployeeFinishedEpic = (action$: ActionsObservable<LoadUserFinished>) =>
    action$.pipe(
        ofType('LOAD-USER-FINISHED'),
        map(x => calculateDaysCounters(x.user))
    );