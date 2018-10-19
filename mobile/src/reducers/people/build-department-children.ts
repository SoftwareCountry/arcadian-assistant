import { DepartmentIdToNode, DepartmentIdToChildren, DepartmentNode, MapDepartmentNode } from './people.model';
import { Map, Set } from 'immutable';

export function buildDepartmentIdToChildren(departmentIdToNode: DepartmentIdToNode): DepartmentIdToChildren {
    const departmentsIds = Object.keys(departmentIdToNode);
    const children: DepartmentIdToChildren = {};

    for (let departmentId of departmentsIds) {
        const node = departmentIdToNode[departmentId];

        if (!node.parentId) {
            continue;
        }

        const child = children[node.parentId];

        if (child) {
            children[node.parentId] = children[node.parentId].add(Map(node));
        } else {
            children[node.parentId] = Set<MapDepartmentNode>([Map(node)]);
        }
    }

    return children;
}