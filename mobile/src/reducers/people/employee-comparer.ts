import { Employee } from '../organization/employee.model';

export class EmployeesComparer {
    public employeesAZSort(first: Employee, second: Employee) {
        if (first.name < first.name) {
            return -1;
        } else if (first.name > first.name) {
            return 1;
        } else {
            return 0;
        }
    }
}
