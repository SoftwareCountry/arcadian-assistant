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
import { FeedsPullingMode } from './feeds.action';
import { ignoreElements } from 'rxjs/operators';

export const pagingPeriodDays = 10;

export const loadFeedsEpic$ = (action$: ActionsObservable<fAction.LoadFeeds>, appState: AppState, deps: DependenciesContainer) =>

    action$.ofType('LOAD_FEEDS')
        .switchMap(x => {
            switch (x.pullingMode) {
                case FeedsPullingMode.OldFeeds:
                    const toDate = x.toDate ? x.toDate.subtract(pagingPeriodDays, 'days') : moment();
                    const fromDate = x.fromDate ? x.fromDate.subtract(pagingPeriodDays, 'days') : moment().subtract(pagingPeriodDays, 'days'); //show news from 10 days before.
                    return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate.format('YYYY-MM-DD')}&toDate=${toDate.format('YYYY-MM-DD')}`)
                        .map((y) => ({ toDate: toDate, fromDate: fromDate, payload: y }));

                case FeedsPullingMode.NewFeeds:
                    const fromDate1 = x.toDate ? x.toDate : moment();
                    const toDate1 = moment();
                    return deps.apiClient.getJSON(`/feeds/messages?fromDate=${fromDate1.format('YYYY-MM-DD')}&toDate=${toDate1.format('YYYY-MM-DD')}`)
                        .map((y) => ({ toDate: toDate1, fromDate: fromDate1, payload: y }));
            }
        })

        .flatMap(x => {
            return Observable.concat(Observable.of(fAction.changeBoundaryDates(x.toDate, x.fromDate)), Observable.of(fAction.loadFeedsFinished(deserializeArray(x.payload as any, Feed))));
        })
        .catch(e => Observable.of(loadFailedError(e.message)));

export const loadFeedsFinishedEpic$ = (action$: ActionsObservable<fAction.LoadFeedsFinished>) =>
    action$.ofType('LOAD_FEEDS_FINISHED')
        .flatMap(x => x.feeds.filter(y => y.employeeId !== null).map(feed =>  oAction.loadEmployee(feed.employeeId)));