import { EmployeeMap } from '../organization/employees.reducer';

export function buildEmployeeNodes(employeesById: EmployeeMap, term: string): EmployeeMap {
    if (!term) {
        return employeesById;
    }

    return employeesById
        .filter(employee => employee.name.toLowerCase().includes(term.toLowerCase()))
        .toMap();
}