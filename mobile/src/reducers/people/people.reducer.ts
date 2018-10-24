import { Reducer } from 'redux';
import { NavigationAction } from 'react-navigation';
import { LoadDepartmentsFinished, LoadEmployeeFinished } from '../organization/organization.action';
import { PeopleActions } from './people.action';
import { SearchActions } from '../search/search.action';
import { SearchType } from '../../navigation/search-view';
import { Department } from '../organization/department.model';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { combineEpics } from 'redux-observable';
import { redirectToEmployeeDetails$ } from './people.epics';
import { MapDepartmentNode, DepartmentNode, EmployeeNode, EmployeeIdToNode } from './people.model';
import { Map, Set } from 'immutable';

export interface PeopleState {
    departmentNodes: Set<MapDepartmentNode>;
    headDepartment: Department;
    filter: string;
    selectedCompanyDepartmentId: string;
    employeeNodes: EmployeeIdToNode;
}

const initState: PeopleState = {
    departmentNodes: Set(),
    headDepartment: null,
    filter: '',
    selectedCompanyDepartmentId: null,
    employeeNodes: Map()
};

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions | NavigationAction | 
        LoadUserEmployeeFinished | LoadDepartmentsFinished | SearchActions | LoadEmployeeFinished): PeopleState => {
    switch (action.type) {
        case 'Navigation/NAVIGATE':
            if (action.routeName === 'Company') {
                if (action.params) {
                    const { departmentId } = action.params;

                    return {
                        ...state,
                        selectedCompanyDepartmentId: departmentId
                    };
                }
            }
            return state;
        case 'LOAD-USER-EMPLOYEE-FINISHED':
            return {
                ...state,
                selectedCompanyDepartmentId: action.employee.departmentId
            };
        case 'LOAD-DEPARTMENTS-FINISHED':
            const departmentNodes: Map<keyof DepartmentNode, DepartmentNode[keyof DepartmentNode]>[] = [];
            const parentIds = Set<string>();

            for (let department of action.departments) {
                const node: DepartmentNode = {
                    departmentId: department.departmentId,
                    parentId: department.parentDepartmentId,
                    abbreviation: department.abbreviation,
                    chiefId: department.chiefId,
                    staffDepartmentId: null
                };

                departmentNodes.push(Map(node));

                if (node.parentId && !parentIds.has(node.parentId)) {
                    const staffNode: DepartmentNode = {
                        departmentId: `[${node.parentId}-staff]`,
                        parentId: node.parentId,
                        abbreviation: null,
                        chiefId: null,
                        staffDepartmentId: node.parentId
                    };
                    departmentNodes.push(Map(staffNode));
                    parentIds.add(node.parentId);
                }
            }

            const headDepartment = action.departments.find(department => department.isHeadDepartment);

            return {
                ...state,
                departmentNodes: Set(departmentNodes),
                headDepartment: headDepartment
            };
        case 'LOAD_EMPLOYEE_FINISHED': {
            const employeeNode: EmployeeNode = {
                employeeId: action.employee.employeeId,
                departmentId: action.employee.departmentId,
                name: action.employee.name,
                position: action.employee.position,
                photoUrl: action.employee.photoUrl
            };

            return {
                ...state,
                employeeNodes: state.employeeNodes.set(employeeNode.employeeId, Map(employeeNode))
            };
        }

        case 'SELECT-COMPANY-DEPARTMENT': 
            return {
                ...state,
                selectedCompanyDepartmentId: action.departmentId
            };

        case 'SEARCH-BY-TEXT-FILTER':
            if (action.searchType === SearchType.People) {
                return {
                    ...state,
                    filter: action.filter,
                };
            }
        default:
            return state;
    }
};

export const peopleEpics = combineEpics(
    redirectToEmployeeDetails$ as any
);