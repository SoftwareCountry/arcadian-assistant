import { ActionsObservable, StateObservable } from 'redux-observable';
import * as fAction from './feeds.action';
import * as oAction from '../organization/organization.action';
import { deserializeArray } from 'santee-dcts/src/deserializer';
import { Feed } from './feed.model';
import { AppState, DependenciesContainer } from '../app.reducer';
import moment from 'moment';
import { filter, flatMap, map, switchMap, withLatestFrom } from 'rxjs/operators';
import { handleHttpErrors } from '../errors/errors.epics';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { concat, of } from 'rxjs';

export const pagingPeriodDays = 10;

export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED').pipe(
        map(x => fAction.fetchNewFeeds()),
    );

export const fetchNewFeedsEpic$ = (action$: ActionsObservable<fAction.FetchNewFeeds>, state$: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('FETCH_NEW_FEEDS').pipe(
        withLatestFrom(state$),
        filter(() => !!state$.value.feeds),
        switchMap(x => {
            const toDate = moment();
            const fromDate = state$.value.feeds!.toDate ? state$.value.feeds!.toDate! : moment().subtract(pagingPeriodDays, 'days');

            return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate.format('YYYY-MM-DD')}&toDate=${toDate.format('YYYY-MM-DD')}`)
                .pipe(
                    handleHttpErrors(),
                    map((y) => ({ fromDate, toDate, payload: y }))
                );
        }),
        flatMap(x => {
            return concat(of(fAction.changeBoundaryDates(x.toDate, x.fromDate)), of(fAction.loadFeedsFinished(deserializeArray(x.payload as any, Feed))));
        }));

export const fetchOldFeedsEpic$ = (action$: ActionsObservable<fAction.FetchOldFeeds>, state$: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('FETCH_OLD_FEEDS').pipe(
        withLatestFrom(state$),
        filter(() => !!state$.value.feeds),
        switchMap(x => {
            const toDate = state$.value.feeds!.fromDate ? state$.value.feeds!.fromDate! : moment();
            const fromDate = state$.value.feeds!.fromDate ? state$.value.feeds!.fromDate!.clone().subtract(pagingPeriodDays, 'days') : moment().subtract(pagingPeriodDays, 'days');

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
        map(x => x.feeds.filter(y => y.employeeId !== null).map(feed => feed.employeeId!)),
        map(employeeIds => oAction.loadEmployees(employeeIds)),
    );
