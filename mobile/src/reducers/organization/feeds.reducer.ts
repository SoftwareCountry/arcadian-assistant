import { Reducer } from 'redux';
import { Feed } from './feed.model';
import { OrganizationActions } from './organization.action';

export const feedsReducer: Reducer<Feed[]> = (state = [], action: OrganizationActions) => {
    switch (action.type) {
        case 'LOAD_FEEDS_FINISHED':
            return [...action.feeds];

        default:
            return state;
    }
};