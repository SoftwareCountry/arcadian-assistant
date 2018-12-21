/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { EmployeeId } from '../reducers/organization/employee.model';
import { NavigateToAction } from './navigation.epic';

//============================================================================
export enum NavigationActionType {
    openEmployeeDetails = 'NavigationActionType.openEmployeeDetails',
    openCompany = 'NavigationActionType.openCompany',
    openDepartment = 'NavigationActionType.openDepartment',
    openRoom = 'NavigationActionType.openRoom',
    openOrganization = 'NavigationActionType.openOrganization',
    openUserPreferences = 'NavigationActionType.openUserPreferences',
    openProfile = 'NavigationActionType.openProfile',
}

//============================================================================
// - Actions
//============================================================================

export interface OpenEmployeeDetailsAction extends NavigateToAction<NavigationActionType.openEmployeeDetails> {
    params: {
        employeeId  : EmployeeId;
    };
}

export interface OpenCompanyAction extends NavigateToAction<NavigationActionType.openCompany> {
    params: {
        departmentId: string
    };
}

export interface OpenDepartmentAction extends NavigateToAction<NavigationActionType.openDepartment> {
    params: {
        departmentId: string;
        departmentAbbreviation: string;
    };
}

export interface OpenRoomAction extends NavigateToAction<NavigationActionType.openRoom> {
    params: {
        roomNumber: string;
    };
}

export interface OpenOrganizationAction extends NavigateToAction<NavigationActionType.openOrganization> {
    params: {
        departmentId: string
    };
}

export interface OpenUserPreferencesAction extends NavigateToAction<NavigationActionType.openUserPreferences> {
}

export interface OpenProfileAction extends NavigateToAction<NavigationActionType.openProfile> {
}


//==============================================================================================
// - Action Creators
//============================================================================

export const openEmployeeDetails = (employeeId: EmployeeId): OpenEmployeeDetailsAction => {
    return {
        type: NavigationActionType.openEmployeeDetails,
        params: {
            employeeId,
        },
    };
};

export const openCompany = (departmentId: string): OpenCompanyAction => {
    return {
        type: NavigationActionType.openCompany,
        params: {
            departmentId,
        },
    };
};

export const openDepartment = (departmentId: string, departmentAbbreviation: string): OpenDepartmentAction => {
    return {
        type: NavigationActionType.openDepartment,
        params: {
            departmentId, departmentAbbreviation
        },
    };
};

export const openRoom = (roomNumber: string): OpenRoomAction => {
    return {
        type: NavigationActionType.openRoom,
        params: {
            roomNumber,
        },
    };
};

export const openOrganization = (departmentId: string): OpenOrganizationAction => {
    return {
        type: NavigationActionType.openOrganization,
        params: {
            departmentId,
        },
    };
};

export const openUserPreferences = (): OpenUserPreferencesAction => {
    return {
        type: NavigationActionType.openUserPreferences,
    };
};

export const openProfile = (): OpenProfileAction => {
    return {
        type: NavigationActionType.openProfile,
    };
};
