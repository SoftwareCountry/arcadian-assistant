import { Action } from 'redux';
import { Feed } from './feed.model';
import moment, { Moment } from 'moment';

export enum FeedsPullingMode {
    OldFeeds = 'OldFeeds',
    NewFeeds = 'NewFeeds',
}
export interface LoadFeeds extends Action {
    type: 'LOAD_FEEDS';
    toDate: Moment;
    fromDate: Moment;
    pullingMode: FeedsPullingMode;
}

export const loadFeeds = (toDate: Moment, fromDate: Moment, pullingMode: FeedsPullingMode): LoadFeeds => ({ type: 'LOAD_FEEDS', toDate, fromDate, pullingMode });

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
    LoadFeeds | LoadFeedsFinished | ChangeBoundaryDates ;