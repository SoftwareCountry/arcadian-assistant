import { Map, Set } from 'immutable';
import { Employee, Photo } from './employee.model';
import { Reducer, combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { OrganizationActions } from './organization.action';
import { loadChiefsEpic$, loadEmployeesForDepartmentEpic$, loadEmployeeEpic$, 
    loadEmployeesForUserDepartmentEpic$, loadEmployeesForUserRoomEpic$, loadEmployeesForRoomEpic$, 
    loadUserEmployeeFinishedEpic$, loadPhotoEpic$ } from './organization.epics';

export type EmployeeMap = Map<string, Employee>;
export type EmployeeIdsGroupMap = Map<string, Set<string>>;
export type EmployeeIdToDepartment = Map<string, string>;
export type PhotoMap = Map<string, Photo>;

export interface EmployeesStore {
    employeesById: EmployeeMap;
    employeeIdsByDepartment: EmployeeIdsGroupMap;
}

export interface OrganizationState {
    employees: EmployeesStore;
    photoById: PhotoMap;
}

const defaultState: EmployeesStore = {
    employeesById: Map(),
    employeeIdsByDepartment: Map()
};

export const employeesReducer: Reducer<EmployeesStore> = (state = defaultState, action: OrganizationActions) => {
    switch (action.type) {
        case 'LOAD_EMPLOYEE_FINISHED':
            let { employeesById, employeeIdsByDepartment } = state;
            
            const newEmployee = action.employee;
            const oldEmployee = employeesById.get(newEmployee.employeeId);
            if (oldEmployee !== undefined) {
                //if department changed, remove a link from old collection
                if (oldEmployee.departmentId !== newEmployee.departmentId) {
                    employeeIdsByDepartment = employeeIdsByDepartment
                        .update(oldEmployee.departmentId, Set(), oldCollection => oldCollection.remove(oldEmployee.employeeId));
                }
            }

            employeeIdsByDepartment = employeeIdsByDepartment
                .update(newEmployee.departmentId, Set(), oldCollection => oldCollection.add(newEmployee.employeeId));

            employeesById = employeesById.set(newEmployee.employeeId, newEmployee);

            return {
                employeeIdsByDepartment,
                employeesById
            };
        default:
            return state;
    }
};

export const photoReducer: Reducer<PhotoMap> = (state = Map(), action: OrganizationActions) => {
    switch (action.type) {
        case 'LOAD_PHOTO_FINISHED':
            return state.set(action.id, action.photo);
        default:
            return state;
    }
};

export const organizationEpics = combineEpics(
    loadEmployeeEpic$ as any,
    loadChiefsEpic$ as any,
    loadEmployeesForDepartmentEpic$ as any,
    loadEmployeesForRoomEpic$ as any,
    loadEmployeesForUserDepartmentEpic$ as any, 
    loadEmployeesForUserRoomEpic$ as any,
    loadUserEmployeeFinishedEpic$ as any,
    loadPhotoEpic$ as any
);

export const organizationReducer = combineReducers<OrganizationState>({
    employees: employeesReducer,
    photoById: photoReducer,
});