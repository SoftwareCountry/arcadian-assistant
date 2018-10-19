import { DepartmentIdToNode, DepartmentIdToSelectedId } from './people.model';

export function buildDepartmentsSelection(
    departmentIdsToNodes: DepartmentIdToNode,
    selectedDepartmentId: string
): DepartmentIdToSelectedId {
    const departmentIdToSelectedId: DepartmentIdToSelectedId = {};
    let selectedDepartment = departmentIdsToNodes[selectedDepartmentId];
    let parent = selectedDepartment ? departmentIdsToNodes[selectedDepartment.parentId] : null;

    while (parent) {
        departmentIdToSelectedId[parent.departmentId] = selectedDepartment.departmentId;
        selectedDepartment = departmentIdsToNodes[parent.departmentId];
        parent = selectedDepartment ? departmentIdsToNodes[selectedDepartment.parentId] : null;
    }

    return departmentIdToSelectedId;
}