import { DepartmentIdToNode, DepartmentIdToSelectedId } from './people.model';

export function buildDepartmentsSelection(
    departmentIdsToNodes: DepartmentIdToNode,
    selectedDepartmentId: string,
    allowSelect: boolean
): DepartmentIdToSelectedId {
    const departmentIdToSelectedId: DepartmentIdToSelectedId = {};
    let selectedDepartment = departmentIdsToNodes[selectedDepartmentId];
    let parent = selectedDepartment ? departmentIdsToNodes[selectedDepartment.parentId] : null;

    while (parent) {
        departmentIdToSelectedId[parent.departmentId] = { 
            selectedDepartmentId: selectedDepartment.departmentId,
            allowSelect: allowSelect
        };
        selectedDepartment = departmentIdsToNodes[parent.departmentId];
        parent = selectedDepartment ? departmentIdsToNodes[selectedDepartment.parentId] : null;
    }

    return departmentIdToSelectedId;
}