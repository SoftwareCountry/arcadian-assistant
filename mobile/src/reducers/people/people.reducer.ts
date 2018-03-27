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
import { DepartmentsTreeNode } from '../../people/departments/departments-tree-node';

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
    departments: null,
    departmentsHeads: null,
    employees: null,
    departmentsHeadsIds: null,
    departmentsBranch: null,
    departmentsTree: null
};

function departmentsTreeFor(departments: Department[], employees: Employee[]) {
    const topLevelDepartment: Department = departments.filter((department) => department.isHeadDepartment === true)[0];
    const topLevelEmployee: Employee = employees.filter((employee) => employee.employeeId === topLevelDepartment.chiefId)[0];
    const departmentsTree: DepartmentsTree = {
        root: {
            departmentId: topLevelDepartment.departmentId,
            departmentAbbreviation: topLevelDepartment.abbreviation,
            departmentChiefId: topLevelDepartment.chiefId,
            head: topLevelEmployee,
            parent: null,
            children: childrenNodes(topLevelDepartment, departments, employees),
            subordinates: employees.filter((employee) => employee.departmentId === topLevelDepartment.departmentId)
        }
    };

    return departmentsTree;
}

function onlyUnique(value: string, index: number, self: string[]) { 
    return self.indexOf(value) === index;
}

function childrenNodes(headDeaprtment: Department, departments: Department[], employees: Employee[]) {
    var nodes: DepartmentsTreeNode[] = [];
    const sublings = departments.filter((subDepartment) => subDepartment.parentDepartmentId === headDeaprtment.departmentId);
    sublings.forEach(department => {
        const employee: Employee = employees.filter((emp) => emp.employeeId === department.chiefId)[0];
        const subSublings = departments.filter((subDepartment) => subDepartment.parentDepartmentId === department.departmentId);
        nodes.push({
            departmentId: department.departmentId,
            departmentAbbreviation: department.abbreviation,
            departmentChiefId: department.chiefId,
            head: employee,
            parent: headDeaprtment,
            children: subSublings.length > 0 ? childrenNodes(department, departments, employees) : null,
            subordinates: employees.filter((emp) => emp.departmentId === department.departmentId)
        });
    });

    return nodes;
}

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions | LoadEmployeeFinished | LoadDepartmentsFinished) => {
    switch (action.type) {
        case 'LOAD_EMPLOYEE_FINISHED':
            var heads = state.departmentsHeads;
            if (heads === null) {
                heads = [];
            }
            if (state.departmentsHeadsIds.indexOf(action.employee.employeeId) > -1 && heads.filter((employee) => employee.employeeId === action.employee.employeeId).length === 0) {
                heads.push(action.employee);
                // console.log(action.employee);
            } else {
                if (heads.length === state.departmentsHeadsIds.length) {
                    var employees: Employee[];
                    if (state.employees === null) {
                        employees = [];
                        employees.push(action.employee);
                    } else {
                        employees = state.employees;
                        if (employees.filter((employee) => employee.employeeId === action.employee.employeeId).length === 0 && state.departmentsHeads.filter((employee) => employee.employeeId === action.employee.employeeId).length === 0) {
                            employees.push(action.employee);
                        } else {
                            return {...state};
                        }
                    }
                    const departmentsTree = departmentsTreeFor(state.departments, state.departmentsHeads.concat(employees));
                    employees.map((employee) => {
                       if (employee.employeeId === '140') {
                           console.log('check duplicates!');
                       } 
                    });
                    return {...state, departmentsTree: departmentsTree, employees: employees};
                } else {
                    return {...state};
                }
            }

            if (heads.length === state.departmentsHeadsIds.length) {
                const departmentsTree = departmentsTreeFor(state.departments, state.employees ? state.departmentsHeads.concat(state.employees) : state.departmentsHeads);
                var currentDepartmentNode = departmentsTree.root;
                var index = 0;
                var initDeps = [];

                while (currentDepartmentNode) {
                    initDeps.push(currentDepartmentNode);
                    const firstChild = currentDepartmentNode.children != null ? currentDepartmentNode.children[0] : null;
                    currentDepartmentNode = firstChild;
                }

                return { ...state, departmentsBranch: initDeps, departmentsTree: departmentsTree };
            }

            return {...state, departmentsHeads: heads};
        case 'LOAD-DEPARTMENTS-FINISHED':
            const headsIds = action.departments.map(department => department.chiefId).filter(onlyUnique);
            const headDepartment = action.departments.filter((department) => department.isHeadDepartment === true)[0];
            return {...state, departments: action.departments, headDepartment: headDepartment, departmentsHeadsIds: headsIds};        
        case 'UPDATE-DEPARTMENT-IDS-TREE':
            var deps = state.departmentsBranch;
            if (deps === null) {
                deps = [];
            }

            if (deps.length - 1 < action.index) {
                deps.push(action.departmentId);
                deps = [].concat(deps);
            } else {
                if (action.departmentId.departmentId === 'subordinates') {
                    deps = deps.filter((dep, depIndex) => depIndex < action.index);
                } else {
                    deps[action.index] = action.departmentId;
                    deps = deps.filter((dep, depIndex) => depIndex < (action.index + 1));
                    var currentDepartmentNodeU = deps[action.index].children != null ? deps[action.index].children[0] : null;
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