import { Employee } from '../reducers/organization/employee.model';
import { NavigateToAction } from './navigation.epic';

export enum NavigationActionType {
    openEmployeeDetails = 'NavigationActionType.openEmployeeDetails',
    openCompany = 'NavigationActionType.openCompany',
    openDepartment = 'NavigationActionType.openDepartment',
    openRoom = 'NavigationActionType.openRoom',
    openOrganization = 'NavigationActionType.openOrganization',
}

//==============================================================================================
export interface OpenEmployeeDetailsAction extends NavigateToAction<NavigationActionType.openEmployeeDetails> {
    params: {
        employee: Employee;
    };
}

//----------------------------------------------------------------------------------------------
export const openEmployeeDetails = (employee: Employee): OpenEmployeeDetailsAction => {
    return {
        type: NavigationActionType.openEmployeeDetails,
        params: {
            employee,
        },
    };
};

//==============================================================================================
export interface OpenCompanyAction extends NavigateToAction<NavigationActionType.openCompany> {
    params: {
        departmentId: string
    };
}

//----------------------------------------------------------------------------------------------
export const openCompany = (departmentId: string): OpenCompanyAction => {
    return {
        type: NavigationActionType.openCompany,
        params: {
            departmentId,
        },
    };
};

//==============================================================================================
export interface OpenDepartmentAction extends NavigateToAction<NavigationActionType.openDepartment> {
    params: {
        departmentId: string;
        departmentAbbreviation: string;
    };
}

//----------------------------------------------------------------------------------------------
export const openDepartment = (departmentId: string, departmentAbbreviation: string): OpenDepartmentAction => {
    return {
        type: NavigationActionType.openDepartment,
        params: {
            departmentId, departmentAbbreviation
        },
    };
};

//==============================================================================================
export interface OpenRoomAction extends NavigateToAction<NavigationActionType.openRoom> {
    params: {
        roomNumber: string;
    };
}

//----------------------------------------------------------------------------------------------
export const openRoom = (roomNumber: string): OpenRoomAction => {
    return {
        type: NavigationActionType.openRoom,
        params: {
            roomNumber,
        },
    };
};

//==============================================================================================
export interface OpenOrganizationAction extends NavigateToAction<NavigationActionType.openOrganization> {
    params: {
        departmentId: string
    };
}

//----------------------------------------------------------------------------------------------
export const openOrganization = (departmentId: string): OpenOrganizationAction => {
    return {
        type: NavigationActionType.openOrganization,
        params: {
            departmentId,
        },
    };
};

