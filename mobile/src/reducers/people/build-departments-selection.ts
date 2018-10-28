import { DepartmentIdToNode, DepartmentIdToSelectedId } from './people.model';

export function buildDepartmentsSelection(
    departmentIdsToNodes: DepartmentIdToNode,
    selectedDepartmentId: string
): DepartmentIdToSelectedId {
    const departmentIdToSelectedId: DepartmentIdToSelectedId = {};
    let selectedDepartment = departmentIdsToNodes.get(selectedDepartmentId);
    let parent = selectedDepartment ? departmentIdsToNodes.get(selectedDepartment.parentId) : null;

    while (parent) {
        departmentIdToSelectedId[parent.departmentId] = selectedDepartment.departmentId;
        selectedDepartment = departmentIdsToNodes.get(parent.departmentId);
        parent = selectedDepartment ? departmentIdsToNodes.get(selectedDepartment.parentId) : null;
    }

    return departmentIdToSelectedId;
}