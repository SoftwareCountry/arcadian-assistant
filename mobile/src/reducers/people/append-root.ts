import { Department } from '../organization/department.model';
import { DepartmentIdToNode } from './people.model';

export const rootId = '[root]';

export function appendRoot(headDepartment: Department | null, departmentIdsToNodes: DepartmentIdToNode) {
    if (!headDepartment) {
        return;
    }

    departmentIdsToNodes[rootId] = {
        departmentId: rootId,
        parentId: null,
        abbreviation: null,
        chiefId: null,
        staffDepartmentId: null
    };

    departmentIdsToNodes[headDepartment.departmentId].parentId = departmentIdsToNodes[rootId].departmentId;
}