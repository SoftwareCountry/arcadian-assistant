import { Action } from 'redux';
import { Feed } from './feed.model';
import { Moment } from 'moment';

export interface FetchNewFeeds extends Action {
    type: 'FETCH_NEW_FEEDS';
}

export const fetchNewFeeds = (): FetchNewFeeds => ({ type: 'FETCH_NEW_FEEDS' });

export interface FetchOldFeeds extends Action {
    type: 'FETCH_OLD_FEEDS';
}

export const fetchOldFeeds = (): FetchOldFeeds => ({ type: 'FETCH_OLD_FEEDS' });

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
    LoadFeedsFinished | ChangeBoundaryDates | FetchNewFeeds | FetchOldFeeds;