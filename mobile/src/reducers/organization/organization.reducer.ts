/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Department } from './department.model';
import { combineEpics } from 'redux-observable';
import { combineReducers } from 'redux';
import { OrganizationState } from './organization.reducer';
import { departmentsReducer } from './departments.reducer';
import { employeesReducer, EmployeesStore } from './employees.reducer';
import {
    handleDepartmentNavigation$,
    loadAllEmployeeEpic$,
    loadChiefsEpic$,
    loadDepartmentsEpic$,
    loadEmployeeEpic$,
    loadEmployeesForDepartmentEpic$,
    loadEmployeesForRoomEpic$,
    loadEmployeesForUserDepartmentEpic$,
    loadEmployeesForUserRoomEpic$,
    loadUserEmployeeFinishedEpic$
} from './organization.epics';

//============================================================================
export interface OrganizationState {
    departments: Department[];
    employees: EmployeesStore;
}

//============================================================================
export const organizationEpics = combineEpics(
    loadEmployeeEpic$ as any,
    loadAllEmployeeEpic$ as any,
    loadDepartmentsEpic$ as any,
    loadChiefsEpic$ as any,
    loadEmployeesForDepartmentEpic$ as any,
    loadEmployeesForRoomEpic$ as any,
    loadEmployeesForUserDepartmentEpic$ as any,
    loadEmployeesForUserRoomEpic$ as any,
    loadUserEmployeeFinishedEpic$ as any,
    handleDepartmentNavigation$ as any
);

//============================================================================
export const organizationReducer = combineReducers<OrganizationState>({
    departments: departmentsReducer,
    employees: employeesReducer
});
