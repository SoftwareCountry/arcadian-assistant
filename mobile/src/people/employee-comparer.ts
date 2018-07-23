import { Employee } from '../reducers/organization/employee.model';

export function employeesAZComparer (first: Employee, second: Employee) {
    if (first.name < second.name) {
        return -1;
    } else if (first.name > second.name) {
        return 1;
    } else {
        return 0;
    }
}