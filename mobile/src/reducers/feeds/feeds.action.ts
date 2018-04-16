import { Action } from 'redux';
import { Feed } from './feed.model';
import moment, { Moment } from 'moment';

export interface LoadFeeds extends Action {
    type: 'LOAD_FEEDS';
}

export const loadFeeds = (): LoadFeeds => ({ type: 'LOAD_FEEDS' });

export interface FetchNewFeeds extends Action {
    type: 'FETCH_NEW_FEEDS';
    upBoundaryDate: Moment;
}

export const fetchNewFeeds = (upBoundaryDate: Moment): FetchNewFeeds => ({ type: 'FETCH_NEW_FEEDS', upBoundaryDate });

export interface FetchOldFeeds extends Action {
    type: 'FETCH_OLD_FEEDS';
    downBoundaryDate: Moment;
}

export const fetchOldFeeds = (downBoundaryDate: Moment): FetchOldFeeds => ({ type: 'FETCH_OLD_FEEDS', downBoundaryDate });

export interface LoadFeedsFinished extends Action {
    type: 'LOAD_FEEDS_FINISHED';
    feeds: Feed[];
}

export const loadFeedsFinished = (feeds: Feed[]): LoadFeedsFinished => {
    return { type: 'LOAD_FEEDS_FINISHED', feeds };
};

export interface ChangeBoundaryDates extends Action {
    type: 'CHANGE_BOUNDARY_DATES';
    toDate: Moment;
    fromDate: Moment;
}

export const changeBoundaryDates = (toDate: Moment, fromDate: Moment): ChangeBoundaryDates => {
    return { type: 'CHANGE_BOUNDARY_DATES', toDate, fromDate };
};


export type FeedsActions =
    LoadFeeds | LoadFeedsFinished | ChangeBoundaryDates | FetchNewFeeds | FetchOldFeeds;