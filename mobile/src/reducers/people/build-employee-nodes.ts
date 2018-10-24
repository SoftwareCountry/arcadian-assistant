import { EmployeeIdToNode } from './people.model';

export function buildEmployeeNodes(employeesById: EmployeeIdToNode, term: string): EmployeeIdToNode {
    if (!term) {
        return employeesById;
    }

    return employeesById
        .filter(employee => (employee.get('name') as string).toLowerCase().includes(term.toLowerCase()))
        .toMap();
}