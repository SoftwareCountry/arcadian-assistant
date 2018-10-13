import { EmployeeMap } from '../organization/employees.reducer';
import { MapEmployeeNode, EmployeeIdToNode, EmployeeNode } from './people.model';
import { Map } from 'immutable';
import { Photo } from '../organization/employee.model';

export function buildEmployeeNodes(employeesById: EmployeeIdToNode, term: string): EmployeeIdToNode {
    if (!term) {
        return employeesById;
    }

    return employeesById
        .filter(employee => (employee.get('name') as string).toLowerCase().includes(term.toLowerCase()))
        .toMap();
}