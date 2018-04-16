import { ActionsObservable, ofType } from 'redux-observable';
import * as fAction from './feeds.action';
import * as oAction from '../organization/organization.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { Feed } from './feed.model';
import { AppState, DependenciesContainer } from '../app.reducer';
import moment, { Moment } from 'moment';
import { changeBoundaryDates } from './feeds.action';

export const pagingPeriodDays = 10;

export const loadFeedsEpic$ = (action$: ActionsObservable<fAction.LoadFeeds>, appState: AppState, deps: DependenciesContainer) =>

    action$.ofType('LOAD_FEEDS')
        .switchMap(x => {
            const fromDate = moment().subtract(pagingPeriodDays, 'days'); //show news from 10 days before.
            const toDate = moment();
            return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate.format('YYYY-MM-DD')}&toDate=${toDate.format('YYYY-MM-DD')}`)
                .map((y) => ({ fromDate: fromDate, toDate: toDate, payload: y }));
        })

        .flatMap(x => {
            return Observable.concat(Observable.of(fAction.changeBoundaryDates(x.toDate, x.fromDate)), Observable.of(fAction.loadFeedsFinished(deserializeArray(x.payload as any, Feed))));
        })
        .catch(e => Observable.of(loadFailedError(e.message)));

export const fetchNewFeedsEpic$ = (action$: ActionsObservable<fAction.FetchNewFeeds>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('FETCH_NEW_FEEDS')
        .switchMap(x => {
            const toDate = moment();
            const fromDate = x.upBoundaryDate ? x.upBoundaryDate : moment().subtract(pagingPeriodDays, 'days');
            return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate.format('YYYY-MM-DD')}&toDate=${toDate.format('YYYY-MM-DD')}`);
        })
        .map(x => deserializeArray(x as any, Feed))
        .map(x => fAction.loadFeedsFinished(x))
        .catch(e => Observable.of(loadFailedError(e.message)));

export const fetchOldFeedsEpic$ = (action$: ActionsObservable<fAction.FetchOldFeeds>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('FETCH_OLD_FEEDS')
        .switchMap(x => {
            const toDate = x.downBoundaryDate ? x.downBoundaryDate : moment();
            const fromDate = x.downBoundaryDate ? x.downBoundaryDate.clone().subtract(pagingPeriodDays, 'days') : moment().subtract(pagingPeriodDays, 'days');
            return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate.format('YYYY-MM-DD')}&toDate=${toDate.format('YYYY-MM-DD')}`);
        })
        .map(x => deserializeArray(x as any, Feed))
        .map(x => fAction.loadFeedsFinished(x))
        .catch(e => Observable.of(loadFailedError(e.message)));


export const loadFeedsFinishedEpic$ = (action$: ActionsObservable<fAction.LoadFeedsFinished>) =>
    action$.ofType('LOAD_FEEDS_FINISHED')
        .flatMap(x => x.feeds.filter(y => y.employeeId !== null).map(feed => oAction.loadEmployee(feed.employeeId)));