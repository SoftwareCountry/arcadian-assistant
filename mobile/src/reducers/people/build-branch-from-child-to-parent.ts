import { Department } from '../organization/department.model';
import { DepartmentIdToNode, DepartmentNode } from './people.model';

export function buildBranchFromChildToParent(
    filteredDepartmentNodes: DepartmentNode[],
    departmentIdsToNodes: DepartmentIdToNode
): DepartmentIdToNode {
    const newDepartmentIdsToNodes: DepartmentIdToNode = {};

    for (let departmentNode of filteredDepartmentNodes) {
        newDepartmentIdsToNodes[departmentNode.departmentId] = departmentIdsToNodes[departmentNode.departmentId];

        let parentDepartment = departmentIdsToNodes[departmentNode.parentId];

        while (parentDepartment) {
            newDepartmentIdsToNodes[parentDepartment.departmentId] = parentDepartment;
            parentDepartment = departmentIdsToNodes[parentDepartment.parentId];
        }
    }

    return newDepartmentIdsToNodes;
}