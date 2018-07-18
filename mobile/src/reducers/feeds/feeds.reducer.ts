import { Reducer } from 'redux';
import { combineEpics } from 'redux-observable';
import { Feed } from './feed.model';
import { FeedsActions } from './feeds.action';
import { SearchActions } from '../search.action';
import { SearchType } from '../../navigation/search-view';
import { loadFeedsFinishedEpic$, pagingPeriodDays, fetchNewFeedsEpic$, fetchOldFeedsEpic$, loadUserEmployeeFinishedEpic$ } from './feeds.epics';
import { Moment } from 'moment';
import { Map } from 'immutable';

export type FeedsById = Map<string, Feed>;

export interface FeedsState {
    feeds: FeedsById;
    toDate: Moment;
    fromDate: Moment;
    filter: string;
}

const initState: FeedsState = {
    feeds: Map<string, Feed>(),
    toDate: null,
    fromDate: null,
    filter: '',
};

export const feedsReducer: Reducer<FeedsState> = (state = initState, action: FeedsActions | SearchActions) => {
    switch (action.type) {
        case 'FETCH_NEW_FEEDS':
        case 'FETCH_OLD_FEEDS':
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
        case 'SEARCH-BY-TEXT-FILTER':
            if (action.searchType === SearchType.Feeds) {
                return {
                    ...state,
                    filter: action.filter,
                };
            }
        default:
            return state;
    }
};

export const feedsEpics = combineEpics(
    loadFeedsFinishedEpic$ as any,
    fetchNewFeedsEpic$ as any,
    fetchOldFeedsEpic$ as any, 
    loadUserEmployeeFinishedEpic$ as any
    
);