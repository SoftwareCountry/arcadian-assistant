import { createStore, combineReducers, Reducer, applyMiddleware } from 'redux';
import { List } from 'immutable';
import { AppState } from './app.reducer';
import { HelpdeskState, helpdeskReducer, helpdeskEpics } from './helpdesk/helpdesk.reducer';
import { navigationReducer } from './navigation.reducer';
import { NavigationState } from 'react-navigation';
import { combineEpics, createEpicMiddleware } from 'redux-observable';
//import { createLogger } from 'redux-logger';
import logger from 'redux-logger';
import { errorsEpics } from './errors/errors.reducer';

import 'rxjs/Rx';
import { OrganizationState, organizationReducer, organizationEpics } from './organization/organization.reducer';
import { UserInfoState, userInfoReducer } from './user/user-info.reducer';
import { userEpics } from './user/user.reducer';
import { FeedsState, feedsReducer, feedsEpics } from './feeds/feeds.reducer';
import { CalendarState, calendarReducer } from './calendar/calendar.reducer';
import { PeopleState, peopleReducer, peopleEpics } from './people/people.reducer';

export interface AppState {
    helpdesk: HelpdeskState;
    organization: OrganizationState;
    nav: NavigationState;
    userInfo: UserInfoState;
    feeds: FeedsState;
    calendar: CalendarState;
    people: PeopleState;
}

const rootEpic = combineEpics(helpdeskEpics as any, organizationEpics as any, errorsEpics as any, userEpics as any, feedsEpics as any, peopleEpics as any);

const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    organization: organizationReducer,
    nav: navigationReducer,
    userInfo: userInfoReducer,
    feeds: feedsReducer,
    calendar: calendarReducer,
    people: peopleReducer
});

export const storeFactory = () => {
    const epicMiddleware = createEpicMiddleware(rootEpic);
    //const loggerMiddleware = createLogger();

    return createStore(reducers, applyMiddleware(epicMiddleware));
};
