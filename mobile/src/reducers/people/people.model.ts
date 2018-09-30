import { Map, Set } from 'immutable';
import { Photo } from '../organization/employee.model';

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

export interface EmployeeNode {
    name: string;
    position: string;
    photo: Map<string, Photo>;
}

export type MapEmployeeNode = Map<keyof EmployeeNode, EmployeeNode[keyof EmployeeNode]>;

export type EmployeeIdToNode = Map<string, MapEmployeeNode>;
