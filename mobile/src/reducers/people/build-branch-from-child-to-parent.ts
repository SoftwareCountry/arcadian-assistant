import { DepartmentIdToNode } from './people.model';

export function buildBranchFromChildToParent(
    filteredDepartmentNodes: DepartmentIdToNode,
    departmentIdToNodes: DepartmentIdToNode
): DepartmentIdToNode {
    const newDepartmentIdsToNodes: DepartmentIdToNode = new Map();

    for (let [, departmentNode] of filteredDepartmentNodes.entries()) {
        newDepartmentIdsToNodes.set(departmentNode.departmentId, departmentIdToNodes.get(departmentNode.departmentId));

        let parentDepartment = departmentIdToNodes.get(departmentNode.parentId);

        while (parentDepartment) {
            newDepartmentIdsToNodes.set(parentDepartment.departmentId, parentDepartment);
            parentDepartment = departmentIdToNodes.get(parentDepartment.parentId);
        }
    }

    return newDepartmentIdsToNodes;
}