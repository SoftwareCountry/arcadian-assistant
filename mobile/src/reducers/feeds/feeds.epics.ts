import { ActionsObservable, ofType } from 'redux-observable';
import * as fAction from './feeds.action';
import * as oAction from '../organization/organization.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { loadFailedError } from '../errors/errors.action';
import { Feed } from './feed.model';
import { AppState, DependenciesContainer } from '../app.reducer';
import moment from 'moment';
import { changeBoundaryDates } from './feeds.action';
import { MiddlewareAPI } from 'redux';
import { Alert } from 'react-native';
import {retryWhen, map, catchError, flatMap, switchMap} from 'rxjs/operators';
import { handleHttpErrors } from '../errors/errors.epics';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { Stack } from 'immutable';
import { concat, Observable, of } from 'rxjs';

export const pagingPeriodDays = 10;

export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED').pipe(
        map(x => fAction.fetchNewFeeds()),
        catchError((e: Error) => of(loadFailedError(e.message))));

export const fetchNewFeedsEpic$ = (action$: ActionsObservable<fAction.FetchNewFeeds>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('FETCH_NEW_FEEDS').pipe(
        switchMap(x => {
            const toDate = moment();
            const fromDate = appState.feeds.toDate ? appState.feeds.toDate : moment().subtract(pagingPeriodDays, 'days');

            return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate.format('YYYY-MM-DD')}&toDate=${toDate.format('YYYY-MM-DD')}`)
                .pipe(
                    handleHttpErrors(),
                    map((y) => ({ fromDate, toDate, payload: y }))
                );
        }),
        flatMap(x => {
            return concat(of(fAction.changeBoundaryDates(x.toDate, x.fromDate)), of(fAction.loadFeedsFinished(deserializeArray(x.payload as any, Feed))));
        }));

export const fetchOldFeedsEpic$ = (action$: ActionsObservable<fAction.FetchOldFeeds>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('FETCH_OLD_FEEDS').pipe(
        switchMap(x => {
            const toDate = appState.feeds.fromDate ? appState.feeds.fromDate : moment();
            const fromDate = appState.feeds.fromDate ? appState.feeds.fromDate.clone().subtract(pagingPeriodDays, 'days') : moment().subtract(pagingPeriodDays, 'days');

            return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate.format('YYYY-MM-DD')}&toDate=${toDate.format('YYYY-MM-DD')}`)
                .pipe(
                    handleHttpErrors(),
                    map((y) => ({ fromDate, toDate, payload: y }))
                );
        }),
        flatMap(x => {
            return concat(of(fAction.changeBoundaryDates(x.toDate, x.fromDate)), of(fAction.loadFeedsFinished(deserializeArray(x.payload as any, Feed))));
        }));

export const loadFeedsFinishedEpic$ = (action$: ActionsObservable<fAction.LoadFeedsFinished>) =>
    action$.ofType('LOAD_FEEDS_FINISHED').pipe(
        flatMap(x => x.feeds.filter(y => y.employeeId !== null).map(feed => oAction.loadEmployee(feed.employeeId))));
