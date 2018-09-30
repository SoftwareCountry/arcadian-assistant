import { Map, Set } from 'immutable';

export interface DepartmentNode {
    departmentId: string;
    parentId: string;
    abbreviation: string;
    chiefId: string;
}

export type MapDepartmentNode = Map<keyof DepartmentNode, DepartmentNode[keyof DepartmentNode]>;

export interface DepartmentIdToChildren {
    [departmentId: string]: Set<MapDepartmentNode>;
}

export interface DepartmentIdToSelectedId {
    [departmentId: string]: string;
}

export type DepartmentIdToNode = { [departmentId: string]: DepartmentNode };
