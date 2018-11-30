import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { Optional } from 'types';

export function getEmployee(state: AppState): Optional<Employee> {
    return (state.organization && state.userInfo && state.userInfo.employeeId) ?
        state.organization.employees.employeesById.get(state.userInfo.employeeId) :
        undefined;
}
