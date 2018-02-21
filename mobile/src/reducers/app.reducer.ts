import { createStore, combineReducers, Reducer, applyMiddleware } from 'redux';
import { List } from 'immutable';
import { AppState } from './app.reducer';
import { HelpdeskState, helpdeskReducer, helpdeskEpics } from './helpdesk/helpdesk.reducer';
import { navigationReducer, peopleNavigationReducer } from './navigation.reducer';
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
import { CalendarState, calendarReducer, calendarEpics } from './calendar/calendar.reducer';
import { peopleEpics } from './people/people.reducer';

export interface AppState {
    helpdesk: HelpdeskState;
    organization: OrganizationState;
    nav: NavigationState;
    peopleNav: NavigationState;
    userInfo: UserInfoState;
    feeds: FeedsState;
    calendar: CalendarState;
}

const rootEpic = combineEpics(helpdeskEpics as any, organizationEpics as any, errorsEpics as any, userEpics as any, feedsEpics as any, calendarEpics as any, peopleEpics as any);

const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    organization: organizationReducer,
    nav: navigationReducer,
    peopleNav: peopleNavigationReducer,
    userInfo: userInfoReducer,
    feeds: feedsReducer,
    calendar: calendarReducer,
});

export const storeFactory = () => {
    const epicMiddleware = createEpicMiddleware(rootEpic);
    //const loggerMiddleware = createLogger();

    return createStore(reducers, applyMiddleware(epicMiddleware));
};
