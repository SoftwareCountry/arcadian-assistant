/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { navigateEpic } from './navigation.epic';
import {
    NavigationActionType,
    OpenCompanyAction,
    OpenDepartmentAction,
    OpenEmployeeDetailsAction,
    OpenOrganizationAction,
    OpenProfileAction,
    OpenRoomAction,
    OpenUserPreferencesAction
} from './navigation.actions';
import { combineEpics } from 'redux-observable';

//----------------------------------------------------------------------------
const openEmployeeDetails$ = navigateEpic<OpenEmployeeDetailsAction>(
    NavigationActionType.openEmployeeDetails,
    'CurrentProfile'
);

//----------------------------------------------------------------------------
const openCompany$ = navigateEpic<OpenCompanyAction>(
    NavigationActionType.openCompany,
    'Company',
);

//----------------------------------------------------------------------------
const openDepartment$ = navigateEpic<OpenDepartmentAction>(
    NavigationActionType.openDepartment,
    'CurrentDepartment'
);

//----------------------------------------------------------------------------
const openRoom$ = navigateEpic<OpenRoomAction>(
    NavigationActionType.openRoom,
    'CurrentRoom'
);

//----------------------------------------------------------------------------
const openOrganization$ = navigateEpic<OpenOrganizationAction>(
    NavigationActionType.openOrganization,
    'CurrentOrganization'
);

//----------------------------------------------------------------------------
const openUserPreferences$ = navigateEpic<OpenUserPreferencesAction>(
    NavigationActionType.openUserPreferences,
    'UserPreferences'
);

//----------------------------------------------------------------------------
const openProfile$ = navigateEpic<OpenProfileAction>(
    NavigationActionType.openProfile,
    'Profile'
);

//----------------------------------------------------------------------------
export const navigationEpics$ = combineEpics(
    openEmployeeDetails$,
    openCompany$,
    openDepartment$,
    openRoom$,
    openOrganization$,
    openUserPreferences$,
    openProfile$
);
