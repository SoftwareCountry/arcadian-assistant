import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { combineEpics } from 'redux-observable';
import { NavigationAction } from 'react-navigation';
import { ActionsObservable } from 'redux-observable';

import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { OrganizationActions, loadEmployeesForDepartment, loadEmployeesForRoom, LoadDepartmentsFinished, LoadEmployeeFinished } from '../organization/organization.action';
import { UserActions, LoadUserEmployeeFinished } from '../user/user.action';
import { PeopleActions } from './people.action';
import { EmployeeMap } from '../organization/employees.reducer';
import { Department } from '../organization/department.model';
import { DepartmentsTree } from '../../people/departments/departments-tree';
import { DepartmentsTreeNode, stubIdForSubordinates } from '../../people/departments/departments-tree-node';

export interface PeopleState {
    headDepartment: Department;
    departments: Department[];
    departmentsHeads: Employee[];
    employees: Employee[];
    departmentsHeadsIds: string[];
    departmentsBranch: DepartmentsTreeNode[];
    departmentsTree: DepartmentsTree;
}

const initState: PeopleState = {
    headDepartment: null,
    departments: [],
    departmentsHeads: [],
    employees: [],
    departmentsHeadsIds: [],
    departmentsBranch: [],
    departmentsTree: null
};

function shortDepartmentsTreeFor(departments: Department[]) {
    const topLevelDepartment: Department = departments.find((department) => department.isHeadDepartment === true);
    const departmentsTree: DepartmentsTree = {
        root: {
            departmentId: topLevelDepartment.departmentId,
            departmentAbbreviation: topLevelDepartment.abbreviation,
            departmentChiefId: topLevelDepartment.chiefId,
            parent: null,
            children: shortChildrenNodes(topLevelDepartment, departments)
        }
    };

    return departmentsTree;
}

function onlyUnique(value: string, index: number, self: string[]) { 
    return self.indexOf(value) === index;
}

function shortChildrenNodes(headDeaprtment: Department, departments: Department[]) {
    const nodes: DepartmentsTreeNode[] = [];
    const siblings = departments.filter((subDepartment) => subDepartment.parentDepartmentId === headDeaprtment.departmentId);

    siblings.forEach(department => {
        const subSiblings = departments.filter((subDepartment) => subDepartment.parentDepartmentId === department.departmentId);
        nodes.push({
            departmentId: department.departmentId,
            departmentAbbreviation: department.abbreviation,
            departmentChiefId: department.chiefId,
            parent: headDeaprtment,
            children: subSiblings.length > 0 ? shortChildrenNodes(department, departments) : null
        });
    });

    return nodes;
}

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions | LoadEmployeeFinished | LoadDepartmentsFinished) => {
    switch (action.type) {
        case 'LOAD-DEPARTMENTS-FINISHED':
            const headsIds = action.departments.map(department => department.chiefId).filter(onlyUnique);
            const headDepartment = action.departments.find((department) => department.isHeadDepartment === true);
            const departmentsTree = shortDepartmentsTreeFor(action.departments);
            let currentDepartmentNode = departmentsTree.root;
            const index = 0;
            const initDeps = [];

            while (currentDepartmentNode) {
                initDeps.push(currentDepartmentNode);
                const firstChild = currentDepartmentNode.children != null ? currentDepartmentNode.children[0] : null;
                currentDepartmentNode = firstChild;
            }
            return {...state, departments: action.departments, headDepartment: headDepartment, departmentsHeadsIds: headsIds, departmentsTree: departmentsTree, departmentsBranch: initDeps};        
        case 'UPDATE-DEPARTMENT-IDS-TREE':
            let deps = state.departmentsBranch;

            if (deps.length - 1 < action.index) {
                deps = [...deps, action.departmentId];
            } else {
                if (action.departmentId.departmentId === stubIdForSubordinates) {
                    deps = deps.filter((dep, depIndex) => depIndex < action.index);
                } else {
                    deps[action.index] = action.departmentId;
                    deps = deps.filter((dep, depIndex) => depIndex < (action.index + 1));
                    let currentDepartmentNodeU = deps[action.index].children != null ? deps[action.index].children[0] : null;
                    while (currentDepartmentNodeU) {
                        deps.push(currentDepartmentNodeU);
                        const firstChild = currentDepartmentNodeU.children != null ? currentDepartmentNodeU.children[0] : null;
                        currentDepartmentNodeU = firstChild;
                    }
                }
            }
            return {...state, departmentsBranch: deps};
        default:
            return state;
    }
};