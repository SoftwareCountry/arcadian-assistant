import { createStore, combineReducers, Reducer } from 'redux';
import { messagesReducer, MessageTemplateViewModel } from './messages.reducer';
import { List } from 'immutable';
import { AppState } from './app.reducer';
import { HelpdeskState } from './helpdesk.reducer';
import { navigationReducer } from './navigation.reducer';
import { NavigationState } from 'react-navigation';

export interface AppState {
    helpdesk: HelpdeskState;
    nav: NavigationState;
}

const reducers = combineReducers<AppState>({
    helpdesk: messagesReducer,
    nav: navigationReducer,
});

export const storeFactory = () => createStore(reducers);