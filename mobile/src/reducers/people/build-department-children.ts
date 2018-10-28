import { DepartmentIdToNode, DepartmentIdToChildren } from './people.model';
import { EmployeeIdsGroupMap } from '../organization/employees.reducer';

export function buildDepartmentIdToChildren(departmentIdToNode: DepartmentIdToNode, employeeIdsByDepartment: EmployeeIdsGroupMap): DepartmentIdToChildren {
    const children: DepartmentIdToChildren = {};

    for (let [, node] of departmentIdToNode.entries()) {

        if (!node.parentId) {
            continue;
        }

        if (!children[node.parentId]) {
            children[node.parentId] = [];
        }

        if (node.staffDepartmentId) {
            const employeesIds = employeeIdsByDepartment.get(node.parentId);
            const parent = departmentIdToNode.get(node.parentId);

            if (!employeesIds 
                || !employeesIds.size 
                || (employeesIds.size === 1 && employeesIds.has(parent.chiefId))) {
                    continue;
            }

            node.abbreviation = `${parent.abbreviation} Staff`;
        }

        children[node.parentId].push(node);
    }

    return children;
}