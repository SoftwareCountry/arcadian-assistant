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

export type MapPhoto = Map<keyof Photo, Photo[keyof Photo]>;

export interface EmployeeNode {
    employeeId: string;
    departmentId: string;
    name: string;
    position: string;
    photoUrl: string;
}

export type MapEmployeeNode = Map<keyof EmployeeNode, EmployeeNode[keyof EmployeeNode]>;

export type EmployeeIdToNode = Map<string, MapEmployeeNode>;
