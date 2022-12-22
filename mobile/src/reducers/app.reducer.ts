/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Action, applyMiddleware, combineReducers, createStore } from 'redux';
import { helpDeskEpics, helpDeskReducer, HelpDeskState } from './help-desk/help-desk.reducer';
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
import { Optional } from 'types';
import { Employee, EmployeeId } from './organization/employee.model';
import { DayModel, defaultDayModel } from './calendar/calendar.model';
import { Storage } from '../storage/storage';
import { Permission } from './user/user-employee-permissions.model';
import { Map } from 'immutable';
import { CalendarEvent } from './calendar/calendar-event.model';
import { EmployeesStore } from './organization/employees.reducer';
import { Department } from './organization/department.model';

// import logger from 'redux-logger';

//============================================================================
export interface AppState {
    helpDesk?: HelpDeskState;
    organization?: OrganizationState;
    userInfo: UserInfoState;
    feeds?: FeedsState;
    calendar?: CalendarState;
    people?: PeopleState;
    authentication?: AuthState;
}

//----------------------------------------------------------------------------
export function hasPermission(permission: Permission, state: AppState): Optional<Boolean> {
    return state.userInfo ?
        state.userInfo.permissions.has(permission) :
        undefined;
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
export function getRequests(state: AppState, employees: Optional<EmployeesStore>): Optional<Map<Employee, CalendarEvent[]>> {
    if (!employees) {
        return undefined;
    }

    const requests = state.calendar ? state.calendar.pendingRequests.requests : undefined;
    return requests ?
        requests
            .filter((event, employeeId) => { return employees.employeesById.get(employeeId) !== undefined; })
            .mapKeys(employeeId => employees.employeesById.get(employeeId)!) :
        undefined;
}

//----------------------------------------------------------------------------
export function areRequestsLoading(state: AppState): boolean {
    return state.calendar ? state.calendar.pendingRequests.requestsAreLoading : true;
}

//----------------------------------------------------------------------------
export function getCalendarEvents(state: AppState): Optional<Map<EmployeeId, CalendarEvent[]>> {
    return state.calendar ? state.calendar.calendarEvents.events : undefined;
}

//----------------------------------------------------------------------------
export function areEventsLoading(state: AppState): boolean {
    return state.calendar ? state.calendar.calendarEvents.eventsAreLoading : true;
}

//----------------------------------------------------------------------------
export function getEmployees(state: AppState): Optional<EmployeesStore> {
    return state.organization ? state.organization.employees : undefined;
}

//----------------------------------------------------------------------------
export function getDepartment(state: AppState, employee: Optional<Employee>): Optional<Department> {
    if (!state.organization || !employee) {
        return undefined;
    }

    const departments = state.organization.departments;
    return departments && employee ?
        departments.find((d) => d.departmentId === employee.departmentId) :
        undefined;
}

//----------------------------------------------------------------------------
const rootEpic = combineEpics(
    helpDeskEpics as any,
    organizationEpics as any,
    userEpics as any,
    feedsEpics as any,
    calendarEpics as any,
    authEpics$ as any,
    refreshEpics as any,
    navigationEpics$ as any);

//----------------------------------------------------------------------------
const reducers = combineReducers<AppState>({
    helpDesk: helpDeskReducer,
    organization: organizationReducer,
    userInfo: userInfoReducer,
    feeds: feedsReducer,
    calendar: calendarReducer,
    people: peopleReducer,
    authentication: authReducer,
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
    storage: Storage;
}

//----------------------------------------------------------------------------
export const storeFactory = (oauthProcess: OAuthProcess, navigationService: NavigationService, storage: Storage) => {
    const dependencies: DependenciesContainer = {
        apiClient: new SecuredApiClient(config.apiUrl, oauthProcess.jwtTokenHandler),
        oauthProcess: oauthProcess,
        navigationService: navigationService,
        storage: storage,
    };

    const epicMiddleware = createEpicMiddleware({ dependencies });
    // const store = createStore(rootReducer, {}, applyMiddleware(epicMiddleware, logger));
    const store = createStore(rootReducer, {}, applyMiddleware(epicMiddleware));

    epicMiddleware.run(rootEpic);

    return store;
};
