import { Action } from 'redux';
import { Feed } from './feed.model';

export interface LoadFeeds extends Action {
    type: 'LOAD_FEEDS';
}

export const loadFeeds = (): LoadFeeds => ({ type: 'LOAD_FEEDS' });

export interface LoadFeedsFinished extends Action {
    type: 'LOAD_FEEDS_FINISHED';
    feeds: Feed[];
}

export const loadFeedsFinished = (feeds: Feed[]): LoadFeedsFinished => {
    return { type: 'LOAD_FEEDS_FINISHED', feeds };
};

export type FeedsActions =
    LoadFeeds | LoadFeedsFinished;