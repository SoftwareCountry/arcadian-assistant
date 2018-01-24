import { LoadDaysCounters, loadDaysFinished, LoadDaysCountersFinished, calculateDaysCounters } from './calendar.action';
import { ActionsObservable, Epic, ofType } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { DaysCountersModel, DaysCounterRaw } from './days-counters.model';
import { map, switchMap } from 'rxjs/operators';

// TODO: load mock
const loadMockDays = (): Observable<DaysCounterRaw> => {
    return Observable.of(
        {
            allVacationDays: { timestamp: 224, title: 'days of vacation left', return: null },
            daysOff: { timestamp: 17, title: 'dayoffs to return', return: true },
        } as DaysCounterRaw
    );
};

export const loadDaysCountersEpic = (action$: ActionsObservable<LoadDaysCounters>) =>
    action$.pipe(
        ofType('LOAD-DAYS-COUNTERS'),
        switchMap(x => loadMockDays()),
        map(x => deserialize(x, DaysCounterRaw)),
        map(x => loadDaysFinished(x))
    );

export const loadDaysFinishedEpic = (action$: ActionsObservable<LoadDaysCountersFinished>) =>
    action$.pipe(
        ofType('LOAD-DAYS-COUNTERS-FINISHED'),
        map(x => calculateDaysCounters(x.daysCounterRaw))
    );