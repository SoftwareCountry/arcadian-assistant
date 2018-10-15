import { Department } from '../organization/department.model';
import { DepartmentIdToNode, DepartmentNode } from './people.model';

export const rootId = '[root]';
export const gmgId = '[GMG Staff]';

export function appendRoot(headDepartment: Department | null, departmentIdsToNodes: DepartmentIdToNode) {
    if (!headDepartment) {
        return;
    }

    const rootNode: DepartmentNode = {
        departmentId: rootId,
        parentId: null,
        abbreviation: null,
        chiefId: null,
        staffDepartmentId: null
    };

    departmentIdsToNodes[rootId] = rootNode;

    departmentIdsToNodes[headDepartment.departmentId].parentId = rootNode.departmentId;
    departmentIdsToNodes[gmgId].parentId = rootNode.departmentId;
}