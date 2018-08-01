import { Map, Set } from 'immutable';
import { Employee, Photo } from './employee.model';
import { Reducer } from 'redux';
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
    photoById: PhotoMap;
}

export interface OrganizationState {
    employees: EmployeesStore;
}

const defaultState: EmployeesStore = {
    employeesById: Map(),
    employeeIdsByDepartment: Map(),
    photoById: Map(),
};

export const employeesReducer: Reducer<EmployeesStore> = (state = defaultState, action: OrganizationActions) => {
    let { employeesById, employeeIdsByDepartment } = state;
    switch (action.type) {
        case 'LOAD_EMPLOYEE_FINISHED':
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
                ...state,
                employeeIdsByDepartment,
                employeesById
            };
        case 'LOAD_PHOTO_FINISHED':
            let employee = employeesById.get(action.id);
            employee.photo = action.photo;
            employeesById = employeesById.set(action.id, employee);
            let { photoById } = state;
            photoById = photoById.set(action.id, action.photo);
            return {
                ...state,
                employeesById,
                photoById,
            };
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