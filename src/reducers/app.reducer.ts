import { createStore, combineReducers, Reducer } from 'redux';
import { List } from 'immutable';
import { AppState } from './app.reducer';
import { HelpdeskState, helpdeskReducer } from './helpdesk/helpdesk.reducer';
import { navigationReducer } from './navigation.reducer';
import { NavigationState } from 'react-navigation';
import { combineEpics } from 'redux-observable';

export interface AppState {
    helpdesk: HelpdeskState;
    nav: NavigationState;
}

const rootEpic = combineEpics()

const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    nav: navigationReducer,
});

export const storeFactory = () => {
    return createStore(reducers);
};
