import { createStore, combineReducers, Reducer, applyMiddleware, Action } from 'redux';
import { List } from 'immutable';
import { AppState } from './app.reducer';
import { HelpdeskState, helpdeskReducer, helpdeskEpics } from './helpdesk/helpdesk.reducer';
import { navigationReducer, peopleNavigationReducer } from './navigation.reducer';
import { NavigationState } from 'react-navigation';
import { combineEpics, createEpicMiddleware, Epic } from 'redux-observable';
//import { createLogger } from 'redux-logger';
import logger from 'redux-logger';
import { errorsEpics } from './errors/errors.reducer';

import 'rxjs/Rx';
import { OrganizationState, organizationReducer, organizationEpics } from './organization/organization.reducer';
import { UserInfoState, userInfoReducer } from './user/user-info.reducer';
import { userEpics } from './user/user.reducer';
import { FeedsState, feedsReducer, feedsEpics } from './feeds/feeds.reducer';
import { CalendarState, calendarReducer, calendarEpics } from './calendar/calendar.reducer';
import { SecuredApiClient } from '../auth/secured-api-client';
import config from '../config';
import { OAuthProcess } from '../auth/oauth-process';

export interface AppState {
    helpdesk: HelpdeskState;
    organization: OrganizationState;
    nav: NavigationState;
    peopleNav: NavigationState;
    userInfo: UserInfoState;
    feeds: FeedsState;
    calendar: CalendarState;
}

const rootEpic = combineEpics(helpdeskEpics as any, organizationEpics as any, errorsEpics as any, userEpics as any, feedsEpics as any, calendarEpics as any);

const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    organization: organizationReducer,
    nav: navigationReducer,
    peopleNav: peopleNavigationReducer,
    userInfo: userInfoReducer,
    feeds: feedsReducer,
    calendar: calendarReducer,
});

export interface DependenciesContainer {
    apiClient: SecuredApiClient;
}

export type AppEpic<T extends Action> = Epic<T, AppState, DependenciesContainer>;

export const storeFactory = (oauthProcess: OAuthProcess) => {
    const dependencies: DependenciesContainer = { apiClient: new SecuredApiClient(config.apiUrl, oauthProcess.authenticationState as any ) };
    const options = { dependencies };
    const epicMiddleware = createEpicMiddleware(rootEpic, options);
    //const loggerMiddleware = createLogger();

    return createStore(reducers, applyMiddleware(epicMiddleware));
};
