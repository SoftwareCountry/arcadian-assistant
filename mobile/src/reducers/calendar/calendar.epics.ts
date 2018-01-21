import { loadDays, LoadDays, loadDaysFinished } from './calendar.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { DaysItem, Days } from './days.model';

// TODO: load mock
const loadMockDays = (): Observable<Days> => {
    return Observable.of(
        {
            vacation: { leftDays: 4, allDays: 28, title: 'days of vacation left' },
            off: { leftDays: 3, allDays: 0, title: 'dayoffs to return' },
            sick: { leftDays: 3, allDays: 7, title: 'days on sick leave' }
        }
    );
};

export const loadDaysEpic$ = (action$: ActionsObservable<LoadDays>) =>
    action$.ofType('LOAD-DAYS')
        .switchMap(x => loadMockDays())
        .map(x => deserialize(x, Days))
        .map(x => loadDaysFinished(x));
