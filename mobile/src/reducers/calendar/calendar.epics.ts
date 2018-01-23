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
            allVacationDays: { timestamp: 224, title: 'days of vacation left' },
            daysOff: { timestamp: 64, title: 'dayoffs to return' },
        } as DaysCountersModel
    );
};

export const loadDaysCountersEpic$ = (action$: ActionsObservable<LoadDaysCounters>) =>
    action$.ofType('LOAD-DAYS-COUNTERS')
        .switchMap(x => loadMockDays())
        .map(x => deserialize(x, DaysCountersModel))
        .map(x => loadDaysFinished(x));
