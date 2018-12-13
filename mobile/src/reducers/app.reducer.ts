import { Action, applyMiddleware, combineReducers, createStore } from 'redux';
import { helpdeskEpics, helpdeskReducer, HelpdeskState } from './helpdesk/helpdesk.reducer';
import { combineEpics, createEpicMiddleware } from 'redux-observable';
//import { createLogger } from 'redux-logger';
import { errorsEpics } from './errors/errors.reducer';

import { organizationEpics, organizationReducer, OrganizationState } from './organization/organization.reducer';
import { userInfoReducer, UserInfoState } from './user/user-info.reducer';
import { userEpics } from './user/user.reducer';
import { feedsEpics, feedsReducer, FeedsState } from './feeds/feeds.reducer';
import { calendarEpics, calendarReducer, CalendarState } from './calendar/calendar.reducer';
import { SecuredApiClient } from '../auth/secured-api-client';
import config from '../config';
import { OAuthProcess } from '../auth/oauth-process';
import { peopleReducer, PeopleState } from './people/people.reducer';
import { authEpics$, authReducer, AuthState } from './auth/auth.reducer';
import { refreshEpics } from './refresh/refresh.reducer';
import { NavigationService } from '../navigation/navigation.service';
import { NavigationDependenciesContainer } from '../navigation/navigation-dependencies-container';
import { navigationEpics$ } from '../navigation/navigation.epics';
import { notifications$ } from '../notifications/notification-listener';

export interface AppState {
    helpdesk?: HelpdeskState;
    organization?: OrganizationState;
    userInfo?: UserInfoState;
    feeds?: FeedsState;
    calendar?: CalendarState;
    people?: PeopleState;
    authentication?: AuthState;
}

const rootEpic = combineEpics(
    helpdeskEpics as any,
    organizationEpics as any,
    errorsEpics as any,
    userEpics as any,
    feedsEpics as any,
    calendarEpics as any,
    authEpics$ as any,
    refreshEpics as any,
    navigationEpics$ as any,
    notifications$ as any);

const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    organization: organizationReducer,
    userInfo: userInfoReducer,
    feeds: feedsReducer,
    calendar: calendarReducer,
    people: peopleReducer,
    authentication: authReducer,

});

const rootReducer = (state: AppState | undefined, action: Action) => {
    if (action.type === 'USER-LOGGED-OUT') {
        return reducers(undefined, action);
    }
    return reducers(state, action);
};

export interface DependenciesContainer extends NavigationDependenciesContainer {
    apiClient: SecuredApiClient;
    oauthProcess: OAuthProcess;
}

export const storeFactory = (oauthProcess: OAuthProcess, navigationService: NavigationService) => {
    const dependencies: DependenciesContainer = {
        apiClient: new SecuredApiClient(config.apiUrl, oauthProcess.authenticationState as any),
        oauthProcess: oauthProcess,
        navigationService: navigationService,
    };

    const epicMiddleware = createEpicMiddleware({ dependencies });
    const store = createStore(rootReducer, {}, applyMiddleware(epicMiddleware));

    epicMiddleware.run(rootEpic);

    return store;
};
