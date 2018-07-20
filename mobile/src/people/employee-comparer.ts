import { Employee } from '../reducers/organization/employee.model';

export function employeesAZSortComparer (first: Employee, second: Employee) {
    if (first.name < second.name) {
        return -1;
    } else if (first.name > second.name) {
        return 1;
    } else {
        return 0;
    }
}