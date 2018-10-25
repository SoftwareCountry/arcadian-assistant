import { Department } from '../organization/department.model';
import { DepartmentIdToNode, DepartmentNode } from './people.model';

export const rootId = '[root]';

export function appendRoot(headDepartment: DepartmentNode | null, departmentIdsToNodes: DepartmentIdToNode) {
    if (!headDepartment) {
        return;
    }

    departmentIdsToNodes[rootId] = new DepartmentNode(
        rootId,
        null,
        null,
        null,
        null
    );

    departmentIdsToNodes[headDepartment.departmentId].parentId = departmentIdsToNodes[rootId].departmentId;
}