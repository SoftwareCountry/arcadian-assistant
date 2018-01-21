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
import { OrganizationState, organizationReducer, organizationEpics } from './organization/organization.reducer';
import { calendarReducer, CalendarState, calendarEpics } from './calendar/calendar.reducer';

export interface AppState {
    helpdesk: HelpdeskState;
    organization: OrganizationState;
    nav: NavigationState;
    calendar: CalendarState;
}

const rootEpic = combineEpics( helpdeskEpics as any, organizationEpics as any, calendarEpics );

const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    organization: organizationReducer,
    nav: navigationReducer,
    calendar: calendarReducer
});

export const storeFactory = () => {
    const epicMiddleware = createEpicMiddleware( rootEpic );
    //const loggerMiddleware = createLogger();

    return createStore(reducers, applyMiddleware( epicMiddleware, logger ));
};
