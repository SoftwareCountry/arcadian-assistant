import { Department } from '../organization/department.model';
import { EmployeeMap, EmployeeIdsGroupMap } from '../organization/employees.reducer';
import { DepartmentNode } from './people.model';

export function filterDepartments(
    departmentNodes: DepartmentNode[],
    employeesById: EmployeeMap
): DepartmentNode[] {
    const employees = employeesById.toArray();
    
    const filteredDepartmentNodes = departmentNodes.filter(departmentNode => {

        for (let employee of employees) {
            const employeeDepartmentId = employee.departmentId;

            if (departmentNode.staffDepartmentId && departmentNode.staffDepartmentId === employeeDepartmentId) {
                return true;
            }

            if (employeeDepartmentId === departmentNode.departmentId) {
                return true;
            }
        }

        return false;
    });

    return filteredDepartmentNodes;
}