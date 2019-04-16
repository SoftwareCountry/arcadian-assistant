/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Action, applyMiddleware, combineReducers, createStore } from 'redux';
import { helpdeskEpics, helpdeskReducer, HelpdeskState } from './helpdesk/helpdesk.reducer';
import { combineEpics, createEpicMiddleware } from 'redux-observable';
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
import { notifications$ } from '../notifications/notification.epics';
import { notificationsReducer, NotificationState } from '../notifications/notifications.reducer';
import { Optional } from 'types';
import { Employee } from './organization/employee.model';
import { JwtTokenHandler } from '../auth/jwt-token-handler';
import { DayModel, defaultDayModel } from './calendar/calendar.model';
import { PinCodeStorage } from '../storage/pin-code-storage';

// import logger from 'redux-logger';

//============================================================================
export interface AppState {
    helpdesk?: HelpdeskState;
    organization?: OrganizationState;
    userInfo?: UserInfoState;
    feeds?: FeedsState;
    calendar?: CalendarState;
    people?: PeopleState;
    authentication?: AuthState;
    notifications: NotificationState;
}

//----------------------------------------------------------------------------
export function getEmployee(state: AppState): Optional<Employee> {
    return (state.organization && state.userInfo && state.userInfo.employeeId) ?
        state.organization.employees.employeesById.get(state.userInfo.employeeId) :
        undefined;
}

//----------------------------------------------------------------------------
export function getStartDay(state: AppState): DayModel {
    if (!state.calendar ||
        !state.calendar.calendarEvents ||
        !state.calendar.calendarEvents.selection ||
        !state.calendar.calendarEvents.selection.interval) {
        return defaultDayModel;
    }

    const selection = state.calendar.calendarEvents.selection;

    if (!selection.interval ||
        !selection.interval.startDay ||
        !selection.interval.endDay ||
        !selection.single ||
        !selection.single.day) {
        return selection.single && selection.single.day ? selection.single.day : defaultDayModel;
    }

    return selection.interval.startDay;
}

//----------------------------------------------------------------------------
export function getEndDay(state: AppState): DayModel {
    if (!state.calendar ||
        !state.calendar.calendarEvents.selection.interval ||
        !state.calendar.calendarEvents.selection ||
        !state.calendar.calendarEvents.selection.interval) {
        return defaultDayModel;
    }

    const selection = state.calendar.calendarEvents.selection;

    if (!selection.interval ||
        !selection.interval.startDay ||
        !selection.interval.endDay ||
        !selection.single ||
        !selection.single.day) {
        return selection.single && selection.single.day ? selection.single.day : defaultDayModel;
    }

    return selection.interval.endDay;
}

//----------------------------------------------------------------------------
const rootEpic = combineEpics(
    helpdeskEpics as any,
    organizationEpics as any,
    userEpics as any,
    feedsEpics as any,
    calendarEpics as any,
    authEpics$ as any,
    refreshEpics as any,
    navigationEpics$ as any,
    notifications$ as any);

//----------------------------------------------------------------------------
const reducers = combineReducers<AppState>({
    helpdesk: helpdeskReducer,
    organization: organizationReducer,
    userInfo: userInfoReducer,
    feeds: feedsReducer,
    calendar: calendarReducer,
    people: peopleReducer,
    authentication: authReducer,
    notifications: notificationsReducer,
});

//----------------------------------------------------------------------------
const rootReducer = (state: AppState | undefined, action: Action) => {
    if (action.type === 'USER-LOGGED-OUT') {
        return reducers(undefined, action);
    }
    return reducers(state, action);
};

//============================================================================
export interface DependenciesContainer extends NavigationDependenciesContainer {
    apiClient: SecuredApiClient;
    oauthProcess: OAuthProcess;
    pinStorage: PinCodeStorage;
}

//----------------------------------------------------------------------------
export const storeFactory = (oauthProcess: OAuthProcess, navigationService: NavigationService, pinStorage: PinCodeStorage) => {
    const dependencies: DependenciesContainer = {
        apiClient: new SecuredApiClient(config.apiUrl, oauthProcess.jwtTokenHandler),
        oauthProcess: oauthProcess,
        navigationService: navigationService,
        pinStorage: pinStorage,
    };

    const epicMiddleware = createEpicMiddleware({ dependencies });
    // const store = createStore(rootReducer, {}, applyMiddleware(epicMiddleware, logger));
    const store = createStore(rootReducer, {}, applyMiddleware(epicMiddleware));

    epicMiddleware.run(rootEpic);

    return store;
};
