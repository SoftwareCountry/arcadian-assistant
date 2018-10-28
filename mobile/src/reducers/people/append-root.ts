import { DepartmentIdToNode, DepartmentNode } from './people.model';

export const rootId = '[root]';

export function appendRoot(headDepartment: DepartmentNode | null, departmentIdsToNodes: DepartmentIdToNode) {
    if (!headDepartment) {
        return;
    }

    departmentIdsToNodes.set(rootId, new DepartmentNode(
        rootId,
        null,
        null,
        null,
        null
    ));

    departmentIdsToNodes.get(headDepartment.departmentId).parentId = rootId;
}