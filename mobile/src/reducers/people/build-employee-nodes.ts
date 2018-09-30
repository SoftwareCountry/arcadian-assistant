import { EmployeeMap } from '../organization/employees.reducer';
import { MapEmployeeNode, EmployeeIdToNode, EmployeeNode } from './people.model';
import { Map } from 'immutable';
import { Photo } from '../organization/employee.model';

export function buildEmployeeNodes(employeesById: EmployeeMap): EmployeeIdToNode {
    return employeesById.map(employee => 
        Map<keyof EmployeeNode, EmployeeNode[keyof EmployeeNode]>({ 
            name: employee.name, 
            position: employee.position, 
            photo: Map<string, Photo>(employee.photo) 
        })
    ).toMap();
}