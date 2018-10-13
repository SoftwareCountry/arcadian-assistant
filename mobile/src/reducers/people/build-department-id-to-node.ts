import { Department } from '../organization/department.model';
import { DepartmentIdToNode, DepartmentNode } from './people.model';

export function buildDepartmentIdToNode(departmentsNodes: DepartmentNode[]): DepartmentIdToNode {
    const departmentIdToNode: DepartmentIdToNode = {};

    for (let departmentNode of departmentsNodes) {
        departmentIdToNode[departmentNode.departmentId] = departmentNode;
    }

    return departmentIdToNode;
}