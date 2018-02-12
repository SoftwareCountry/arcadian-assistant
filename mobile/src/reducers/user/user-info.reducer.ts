import { Map, Set } from 'immutable';
import { User } from './user.model';
import { Employee } from '../organization/employee.model';
import { Reducer } from 'redux';
import { OrganizationActions } from '../organization/organization.action';
import { UserActions } from './user.action';
import { EmployeeMap } from '../organization/employees.reducer';

export interface UserInfoState {
    employee: Employee;
    employees: EmployeeMap;
}

const initState: UserInfoState = {
    employee: null,
    employees: Map()
};

export const userInfoReducer: Reducer<UserInfoState> = (state = initState, action: UserActions) => {
    switch (action.type) {
        case 'LOAD-USER-EMPLOYEE-FINISHED':
            return {
                ...state,
                employee: action.employee
            };
        case 'LOAD-USER-DEPARTMENT-EMPOYEES-FINISHED':
            let { employees} = state;

            return {
                ...state,
                employees: employees.set(action.employee.employeeId, action.employee)
            };
        default:
            return state;
    }
};