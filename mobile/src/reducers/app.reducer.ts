import { createStore, combineReducers, Reducer, applyMiddleware, Action, Middleware } from 'redux';
import { List } from 'immutable';
import { AppState } from './app.reducer';
import { HelpdeskState, helpdeskReducer, helpdeskEpics } from './helpdesk/helpdesk.reducer';
import { navigationReducer, navigationMiddlewareKeyReducer } from './navigation.reducer';
import { NavigationState } from 'react-navigation';
import { combineEpics, createEpicMiddleware, Epic } from 'redux-observable';
//import { createLogger } from 'redux-logger';
import logger from 'redux-logger';
import { errorsEpics } from './errors/errors.reducer';

import 'rxjs/Rx';
import { OrganizationState, organizationReducer, organizationEpics } from './organization/employees.reducer';
import { UserInfoState, userInfoReducer } from './user/user-info.reducer';
import { userEpics } from './user/user.reducer';
import { FeedsState, feedsReducer, feedsEpics } from './feeds/feeds.reducer';
import { CalendarState, calendarReducer, calendarEpics } from './calendar/calendar.reducer';
import { SecuredApiClient } from '../auth/secured-api-client';
import config from '../config';
import { OAuthProcess } from '../auth/oauth-process';
import { createReactNavigationReduxMiddleware, createReduxBoundAddListener } from 'react-navigation-redux-helpers';
import { PeopleState, peopleReducer, peopleEpics  } from './people/people.reducer';
import { authEpics$, authReducer, AuthState } from './auth/auth.reducer';
import { refreshEpics } from './refresh/refresh.reducer';

export interface AppState {
    helpdesk?: HelpdeskState;
    organization?: OrganizationState;
    
    nav?: NavigationState;
    navigationMiddlewareKey: string;

    userInfo?: UserInfoState;
    feeds?: FeedsState;
    calendar?: CalendarState;
    people?: PeopleState;

    authentication?: AuthState;  
}

const rootEpic = combineEpics(
    helpdeskEpics as any, 
    organizationEpics as any, 
    peopleEpics as any,
    errorsEpics as any, 
    userEpics as any, 
    feedsEpics as any, 
    calendarEpics as any, 
    authEpics$ as any, 
    refreshEpics as any);

const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    organization: organizationReducer,
    nav: navigationReducer,
    navigationMiddlewareKey: navigationMiddlewareKeyReducer,
    userInfo: userInfoReducer,
    feeds: feedsReducer,
    calendar: calendarReducer,
    people: peopleReducer,
    authentication: authReducer,

});

const rootReducer = (state: AppState, action: Action) => {
    if (action.type === 'USER-LOGGED-OUT') {
      state = undefined;
    }
    return reducers(state, action);
  };

export interface DependenciesContainer {
    apiClient: SecuredApiClient;
    oauthProcess: OAuthProcess;
}

export type AppEpic<T extends Action> = Epic<T, AppState, DependenciesContainer>;

export const storeFactory = (oauthProcess: OAuthProcess, ) => {
    const dependencies: DependenciesContainer = { apiClient: new SecuredApiClient(config.apiUrl,  oauthProcess.authenticationState as any ), oauthProcess: oauthProcess };
    const options = { dependencies };
    const epicMiddleware = createEpicMiddleware(rootEpic, options);

    const navigationMiddlewareKey = 'root';

    const reactNavigationMiddleware = createReactNavigationReduxMiddleware<AppState>(navigationMiddlewareKey, (state) => state.nav);

    return createStore<AppState>(rootReducer, { navigationMiddlewareKey }, applyMiddleware(epicMiddleware, reactNavigationMiddleware));
};
