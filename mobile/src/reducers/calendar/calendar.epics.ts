import { loadDaysCounters, LoadDaysCounters, loadDaysFinished } from './calendar.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { DaysCounterItem, DaysCountersModel } from './days-counters.model';

// TODO: load mock
const loadMockDays = (): Observable<DaysCountersModel> => {
    return Observable.of(
        {
            vacation: { leftDays: 4, allDays: 28, title: 'days of vacation left' },
            off: { leftDays: 3, allDays: 0, title: 'dayoffs to return' },
            sick: { leftDays: 3, allDays: 7, title: 'days on sick leave' }
        }
    );
};

export const loadDaysCountersEpic$ = (action$: ActionsObservable<LoadDaysCounters>) =>
    action$.ofType('LOAD-DAYS-COUNTERS')
        .switchMap(x => loadMockDays())
        .map(x => deserialize(x, DaysCountersModel))
        .map(x => loadDaysFinished(x));
