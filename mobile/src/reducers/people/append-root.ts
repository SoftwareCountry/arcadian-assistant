import { DepartmentIdToNode, DepartmentNode } from './people.model';
import { Nullable } from 'types';

export const rootId = '[root]';

export function appendRoot(headDepartment: Nullable<DepartmentNode>, departmentIdsToNodes: DepartmentIdToNode) {
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

    const node = departmentIdsToNodes.get(headDepartment.departmentId);
    if (node) {
        node.parentId = rootId;
    }
}
