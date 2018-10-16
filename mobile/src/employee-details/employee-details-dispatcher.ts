import { NavigationActions } from 'react-navigation';
import { Dispatch } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';

export const openEmployeeDetailsAction = (employee: Employee) => NavigationActions.navigate({
    routeName: 'CurrentProfile',
    params: {employee},
});

export const openCompanyAction = (departmentId: string) => NavigationActions.navigate({
    routeName: 'Company',
    params: {departmentId},
});

export interface CurrentDepartmentNavigationParams {
    departmentId: string;
}

export const openDepartmentAction = (departmentId: string) => NavigationActions.navigate({
    routeName: 'CurrentDepartment',
    params: { departmentId } as CurrentDepartmentNavigationParams
});

export interface CurrentRoomNavigationParams {
    roomNumber: string;
}

export const openRoomAction = (roomNumber: string) => NavigationActions.navigate({
    routeName: 'CurrentRoom',
    params: { roomNumber } as CurrentRoomNavigationParams
});

export interface CurrentOrganizationNavigationParams {
    departmentId: string;
}

export const openOrganizationAction = (departmentId: string) => NavigationActions.navigate({
    routeName: 'CurrentOrganization',
    params: { departmentId } as CurrentOrganizationNavigationParams
});