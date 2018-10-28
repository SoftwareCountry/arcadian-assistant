import { Department } from '../organization/department.model';
import { EmployeeMap, EmployeeIdsGroupMap } from '../organization/employees.reducer';
import {  DepartmentIdToNode } from './people.model';

export function filterDepartments(
    departmentIdToNode: DepartmentIdToNode,
    employeesById: EmployeeMap
): DepartmentIdToNode {
    const employees = employeesById.toArray();
    const array = Array.from(departmentIdToNode.entries());

    const filteredDepartmentNodes = array.filter(([, departmentNode]) => {

        for (let employee of employees) {
            if (departmentNode.staffDepartmentId && departmentNode.staffDepartmentId === employee.departmentId) {
                return true;
            }

            if (employee.departmentId === departmentNode.departmentId) {
                return true;
            }
        }

        return false;
    });

    return new Map(filteredDepartmentNodes);
}