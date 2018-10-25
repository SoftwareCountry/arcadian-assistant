import { Reducer } from 'redux';
import { NavigationAction } from 'react-navigation';
import { LoadDepartmentsFinished, LoadEmployeeFinished } from '../organization/organization.action';
import { PeopleActions } from './people.action';
import { SearchActions } from '../search/search.action';
import { SearchType } from '../../navigation/search-view';
import { Department } from '../organization/department.model';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { combineEpics } from 'redux-observable';
import { DepartmentNode } from './people.model';
import { Map, Set } from 'immutable';

export interface PeopleState {
    departmentNodes: DepartmentNode[];
    headDepartment: DepartmentNode;
    filter: string;
    selectedCompanyDepartmentId: string;
}

const initState: PeopleState = {
    departmentNodes: [],
    headDepartment: null,
    filter: '',
    selectedCompanyDepartmentId: null
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
            const departmentNodes: DepartmentNode[] = [];
            const parentIds = Set<string>();
            let headDepartment: DepartmentNode = null;

            for (let department of action.departments) {
                const node: DepartmentNode = new DepartmentNode(
                    department.departmentId,
                    department.parentDepartmentId,
                    department.abbreviation,
                    department.chiefId,
                    null
                );

                departmentNodes.push(node);

                if (!headDepartment && department.isHeadDepartment) {
                    headDepartment = node;
                }                

                if (node.parentId && !parentIds.has(node.parentId)) {
                    const staffNode: DepartmentNode = new DepartmentNode(
                        `[${node.parentId}-staff]`,
                        node.parentId,
                        null,
                        null,
                        node.parentId
                    );
                    departmentNodes.push(staffNode);
                    parentIds.add(node.parentId);
                }
            }

            return {
                ...state,
                departmentNodes: departmentNodes,
                headDepartment: headDepartment
            };

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