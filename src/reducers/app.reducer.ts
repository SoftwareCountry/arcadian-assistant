import { createStore, combineReducers, Reducer, applyMiddleware } from 'redux';
import { List } from 'immutable';
import { AppState } from './app.reducer';
import { HelpdeskState, helpdeskReducer, helpdeskEpics } from './helpdesk/helpdesk.reducer';
import { navigationReducer } from './navigation.reducer';
import { NavigationState } from 'react-navigation';
import { combineEpics, createEpicMiddleware } from 'redux-observable';

import 'rxjs/Rx';

export interface AppState {
    helpdesk: HelpdeskState;
    nav: NavigationState;
}

const rootEpic = combineEpics( helpdeskEpics );

const epicMiddleware = createEpicMiddleware( rootEpic );

const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    nav: navigationReducer,
});

export const storeFactory = () => {
    return createStore(reducers, applyMiddleware( epicMiddleware ));
};
