import { Department } from '../organization/department.model';
import { DepartmentIdToNode } from './people.model';

export function buildBranchFromChildToParent(
    filteredDepartments: Department[],
    departmentIdsToNodes: DepartmentIdToNode
): DepartmentIdToNode {
    const newDepartmentIdsToNodes: DepartmentIdToNode = {};

    for (let department of filteredDepartments) {
        newDepartmentIdsToNodes[department.departmentId] = departmentIdsToNodes[department.departmentId];

        let parentDepartment = departmentIdsToNodes[department.parentDepartmentId];

        while (parentDepartment) {
            newDepartmentIdsToNodes[parentDepartment.departmentId] = parentDepartment;
            parentDepartment = departmentIdsToNodes[parentDepartment.parentId];
        }
    }

    return newDepartmentIdsToNodes;
}