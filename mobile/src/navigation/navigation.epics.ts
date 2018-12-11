import { navigationEpic } from './navigation.epic';
import {
    NavigationActionType,
    OpenCompanyAction,
    OpenDepartmentAction,
    OpenEmployeeDetailsAction,
    OpenOrganizationAction,
    OpenRoomAction,
    OpenUserPreferencesAction
} from './navigation.actions';
import { combineEpics } from 'redux-observable';

//----------------------------------------------------------------------------
const openEmployeeDetails$ = navigationEpic<OpenEmployeeDetailsAction>(
    NavigationActionType.openEmployeeDetails,
    'CurrentProfile'
);

//----------------------------------------------------------------------------
const openCompany$ = navigationEpic<OpenCompanyAction>(
    NavigationActionType.openCompany,
    'Company',
);

//----------------------------------------------------------------------------
const openDepartment$ = navigationEpic<OpenDepartmentAction>(
    NavigationActionType.openDepartment,
    'CurrentDepartment'
);

//----------------------------------------------------------------------------
const openRoom$ = navigationEpic<OpenRoomAction>(
    NavigationActionType.openRoom,
    'CurrentRoom'
);

//----------------------------------------------------------------------------
const openOrganization$ = navigationEpic<OpenOrganizationAction>(
    NavigationActionType.openOrganization,
    'CurrentOrganization'
);

//----------------------------------------------------------------------------
const openUserPreferences$ = navigationEpic<OpenUserPreferencesAction>(
    NavigationActionType.openUserPreferences,
    'UserPreferences'
);

//----------------------------------------------------------------------------
export const navigationEpics$ = combineEpics(
    openEmployeeDetails$, openCompany$, openDepartment$, openRoom$, openOrganization$, openUserPreferences$
);
