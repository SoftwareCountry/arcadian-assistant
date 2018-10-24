import { DepartmentIdToNode, DepartmentIdToChildren, DepartmentNode, MapDepartmentNode } from './people.model';
import { Map, Set } from 'immutable';
import { EmployeeIdsGroupMap } from '../organization/employees.reducer';

export function buildDepartmentIdToChildren(departmentIdToNode: DepartmentIdToNode, employeeIdsByDepartment: EmployeeIdsGroupMap): DepartmentIdToChildren {
    const departmentsIds = Object.keys(departmentIdToNode);
    const children: DepartmentIdToChildren = {};

    for (let departmentId of departmentsIds) {
        const node = departmentIdToNode[departmentId];

        if (!node.parentId) {
            continue;
        }

        if (!children[node.parentId]) {
            children[node.parentId] = Set<MapDepartmentNode>();
        }

        if (node.staffDepartmentId) {
            const employeesIds = employeeIdsByDepartment.get(node.parentId);
            const parent = departmentIdToNode[node.parentId];

            if (!employeesIds 
                || !employeesIds.size 
                || (employeesIds.size === 1 && employeesIds.has(parent.chiefId))) {
                    continue;
            }

            node.abbreviation = `${parent.abbreviation} Staff`;
        }

        children[node.parentId] = children[node.parentId].add(Map(node));
    }

    return children;
}