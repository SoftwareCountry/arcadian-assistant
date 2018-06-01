import { ActionsObservable, ofType } from 'redux-observable';
import * as fAction from './feeds.action';
import * as oAction from '../organization/organization.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { Feed } from './feed.model';
import { AppState, DependenciesContainer } from '../app.reducer';
import moment from 'moment';
import { changeBoundaryDates } from './feeds.action';
import { MiddlewareAPI } from 'redux';
import { Alert } from 'react-native';
import { retryWhen, map } from 'rxjs/operators';
import { UnaryFunction } from 'rxjs/interfaces';
import { handleHttpErrors } from '../errors/errors.epics';
import { LoadUserEmployeeFinished } from '../user/user.action';

export const pagingPeriodDays = 10;

export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        .map(x => fAction.fetchNewFeeds())
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const fetchNewFeedsEpic$ = (action$: ActionsObservable<fAction.FetchNewFeeds>, appState: MiddlewareAPI<AppState>, deps: DependenciesContainer) =>
    action$.ofType('FETCH_NEW_FEEDS')
        .switchMap(x => {
            const toDate = moment();
            const fromDate = appState.getState().feeds.toDate ? appState.getState().feeds.toDate : moment().subtract(pagingPeriodDays, 'days');
            return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate.format('YYYY-MM-DD')}&toDate=${toDate.format('YYYY-MM-DD')}`)
                .pipe(
                    handleHttpErrors(),
                    map((y) => ({ fromDate: appState.getState().feeds.fromDate, toDate: toDate, payload: y }))
                );
        })
        .flatMap(x => {
            return Observable.concat(Observable.of(fAction.changeBoundaryDates(x.toDate, x.fromDate)), Observable.of(fAction.loadFeedsFinished(deserializeArray(x.payload as any, Feed))));
        });

export const fetchOldFeedsEpic$ = (action$: ActionsObservable<fAction.FetchOldFeeds>, appState: MiddlewareAPI<AppState>, deps: DependenciesContainer) =>
    action$.ofType('FETCH_OLD_FEEDS')
        .switchMap(x => {
            const toDate = appState.getState().feeds.fromDate ? appState.getState().feeds.fromDate : moment();
            const fromDate = appState.getState().feeds.fromDate ? appState.getState().feeds.fromDate.clone().subtract(pagingPeriodDays, 'days') : moment().subtract(pagingPeriodDays, 'days');

            return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate.format('YYYY-MM-DD')}&toDate=${toDate.format('YYYY-MM-DD')}`)
                .pipe(
                    handleHttpErrors(),
                    map((y) => ({ fromDate: fromDate, toDate: appState.getState().feeds.toDate, payload: y }))
                );
        })
        .flatMap(x => {
            return Observable.concat(Observable.of(fAction.changeBoundaryDates(x.toDate, x.fromDate)), Observable.of(fAction.loadFeedsFinished(deserializeArray(x.payload as any, Feed))));
        });

export const loadFeedsFinishedEpic$ = (action$: ActionsObservable<fAction.LoadFeedsFinished>) =>
    action$.ofType('LOAD_FEEDS_FINISHED')
        .flatMap(x => x.feeds.filter(y => y.employeeId !== null).map(feed => oAction.loadEmployee(feed.employeeId)));