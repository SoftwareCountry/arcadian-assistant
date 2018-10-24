import { Department } from '../organization/department.model';
import { EmployeeMap, EmployeeIdsGroupMap } from '../organization/employees.reducer';
import { EmployeeIdToNode, DepartmentNode } from './people.model';

export function filterDepartments(
    departmentNodes: DepartmentNode[],
    employeeIdToNode: EmployeeIdToNode
): DepartmentNode[] {
    const employeeNodes = employeeIdToNode.toArray();

    const filteredDepartmentNodes = departmentNodes.filter(departmentNode => {

        for (let employeeNode of employeeNodes) {
            const employeeDepartmentId = employeeNode.get('departmentId');

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