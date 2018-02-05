import { Reducer } from 'redux';
import { combineEpics } from 'redux-observable';
import { Feed } from './feed.model';
import { FeedsActions } from './feeds.action';
import { loadFeedsEpic$, loadFeedsFinishedEpic$ } from './feeds.epics';

export const feedsReducer: Reducer<Feed[]> = (state = [], action: FeedsActions) => {
    switch (action.type) {
        case 'LOAD_FEEDS_FINISHED':
            return [...action.feeds];

        default:
            return state;
    }
};

export interface FeedsState extends Array<Feed> { }

export const feedsEpics = combineEpics(
    loadFeedsEpic$ as any,
    loadFeedsFinishedEpic$ as any
);