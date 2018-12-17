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

export const defaultState: EmployeesStore = {
    employeesById: Map(),
    employeeIdsByDepartment: Map()
};

export const employeesReducer: Reducer<EmployeesStore> = (state = defaultState, action: OrganizationActions) => {
    switch (action.type) {
        case 'LOAD_EMPLOYEES_FINISHED':
            let { employeesById, employeeIdsByDepartment } = state;

            for (const newEmployee of action.employees) {
                const oldEmployee = employeesById.get(newEmployee.employeeId);
                if (oldEmployee !== undefined) {

                    if (oldEmployee.equals(newEmployee)) {
                        continue;
                    }

                    //if department changed, remove a link from old collection
                    if (oldEmployee.departmentId !== newEmployee.departmentId) {
                        employeeIdsByDepartment = employeeIdsByDepartment
                            .update(oldEmployee.departmentId, Set(), oldSet => oldSet.remove(oldEmployee.employeeId));
                    }
                }

                employeeIdsByDepartment = employeeIdsByDepartment
                    .update(newEmployee.departmentId, Set(), oldSet => oldSet.add(newEmployee.employeeId));

                employeesById = employeesById.set(newEmployee.employeeId, newEmployee);
            }

            return {
                employeeIdsByDepartment,
                employeesById,
            };

        default:
            return state;
    }
};
