import { EmployeeMap } from '../organization/employees.reducer';
import { MapEmployeeNode, EmployeeIdToNode, EmployeeNode } from './people.model';
import { Map } from 'immutable';
import { Photo } from '../organization/employee.model';

export function buildEmployeeNodes(employeesById: EmployeeMap): EmployeeIdToNode {
    return employeesById.map(employee => {
        const employeeNode: EmployeeNode = {
            employeeId: employee.employeeId,
            departmentId: employee.departmentId,
            name: employee.name,
            position: employee.position,
            photo: Map<string, Photo>(employee.photo)
        };

        return Map<keyof EmployeeNode, EmployeeNode[keyof EmployeeNode]>(employeeNode);
    }).toMap();
}