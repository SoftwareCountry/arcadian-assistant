import { Department } from '../organization/department.model';
import { DepartmentIdToNode } from './people.model';

export function buildDepartmentIdToNode(departments: Department[]): DepartmentIdToNode {
    const departmentIdToNode: DepartmentIdToNode = {};

    for (let department of departments) {
        departmentIdToNode[department.departmentId] = {
            departmentId: department.departmentId,
            parentId: department.parentDepartmentId,
            abbreviation: department.abbreviation,
            chiefId: department.chiefId
        };
    }

    return departmentIdToNode;
}