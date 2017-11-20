import { createStore, combineReducers, Reducer, applyMiddleware } from 'redux';
import { List } from 'immutable';
import { AppState } from './app.reducer';
import { HelpdeskState, helpdeskReducer, helpdeskEpics } from './helpdesk/helpdesk.reducer';
import { navigationReducer } from './navigation.reducer';
import { NavigationState } from 'react-navigation';
import { combineEpics, createEpicMiddleware } from 'redux-observable';
//import { createLogger } from 'redux-logger';
import logger from 'redux-logger';

import 'rxjs/Rx';

export interface AppState {
    helpdesk: HelpdeskState;
    nav: NavigationState;
}

const rootEpic = combineEpics( helpdeskEpics );

const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    nav: navigationReducer,
});

export const storeFactory = () => {
    const epicMiddleware = createEpicMiddleware( rootEpic );
    //const loggerMiddleware = createLogger();

    return createStore(reducers, applyMiddleware( epicMiddleware, logger ));
};
