import { Reducer } from 'redux';
import { combineEpics } from 'redux-observable';
import { Feed } from './feed.model';
import { FeedsActions } from './feeds.action';
import { loadFeedsEpic$, loadFeedsFinishedEpic$, pagingPeriodDays } from './feeds.epics';
import moment, { Moment } from 'moment';
import { Map } from 'immutable';

export type FeedsById = Map<string, Feed>;

export interface FeedsState {
    feeds: FeedsById;
    toDate: Moment;
    fromDate: Moment;
}

const initState: FeedsState = {
    feeds: Map<string, Feed>(),
    toDate: null,
    fromDate: null,
};

export const feedsReducer: Reducer<FeedsState> = (state = initState, action: FeedsActions) => {
    switch (action.type) {
        case 'LOAD_FEEDS':
            return {
                ...state
            };
        case 'LOAD_FEEDS_FINISHED':

            let feeds = state.feeds;

            for (const feed of action.feeds) {
                feeds = feeds.set(feed.messageId, feed);
            }

            return {
                ...state,
                feeds,
            };
        case 'CHANGE_BOUNDARY_DATES':
            return {
                ...state,
                toDate: action.toDate,
                fromDate: action.fromDate,
            };
        default:
            return state;
    }
};

export const feedsEpics = combineEpics(
    loadFeedsEpic$ as any,
    loadFeedsFinishedEpic$ as any
);