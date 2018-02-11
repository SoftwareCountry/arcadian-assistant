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
    if (feeds && feeds.length === 1) {
        //TODO: remove mock data when there will be multiple messages
        const feed = feeds[0];
        feeds = Array.apply(0, Array(5)).map(function (f: void, i: number) { return Object.assign({}, feed, { messageId: i, title: `${feed.title} ${i}` }); });
    }
    return { type: 'LOAD_FEEDS_FINISHED', feeds };
};

export type FeedsActions =
    LoadFeeds | LoadFeedsFinished;