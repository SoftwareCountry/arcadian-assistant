import { Map, Set } from 'immutable';
import { Employee } from './employee.model';
import { Reducer } from 'redux';
import { OrganizationActions } from './organization.action';

export type EmployeeMap = Map<string, Employee>;
export type EmployeeIdsGroupMap = Map<string, Set<string>>;
export type EmployeeIdToDepartment = Map<string, string>;

export interface EmployeesStore {
    employeesById: EmployeeMap;
    employeeIdsByDepartment: EmployeeIdsGroupMap;
}

const defaultState: EmployeesStore = {
    employeesById: Map(),
    employeeIdsByDepartment: Map()
};

export const employeesReducer: Reducer<EmployeesStore> = (state = defaultState, action: OrganizationActions) => {
    switch (action.type) {
        case 'LOAD_EMPLOYEE_FINISHED':
            let {employeesById, employeeIdsByDepartment} = state;

            const newEmployee = action.employee;
            const oldEmployee = employeesById.get(newEmployee.employeeId);
            if (oldEmployee !== undefined) {
                //if department changed, remove a link from old collection
                if (oldEmployee.departmentId !== newEmployee.departmentId) {
                    employeeIdsByDepartment = employeeIdsByDepartment
                        .update(oldEmployee.departmentId, Set(), oldColelction => oldColelction.remove(oldEmployee.employeeId));
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