import { Reducer } from 'redux';
import { NavigationAction } from 'react-navigation';
import { LoadDepartmentsFinished, LoadEmployeeFinished } from '../organization/organization.action';
import { PeopleActions } from './people.action';
import { SearchActions } from '../search/search.action';
import { SearchType } from '../../navigation/search-view';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { DepartmentNode, DepartmentIdToNode } from './people.model';
import { appendRoot } from './append-root';

export interface PeopleState {
    departmentIdToNodes: DepartmentIdToNode;
    headDepartment: DepartmentNode;
    filter: string;
    isFilterActive: boolean;
    selectedCompanyDepartmentId: string;
}

const initState: PeopleState = {
    departmentIdToNodes: new Map<string, DepartmentNode>(),
    headDepartment: null,
    filter: '',
    isFilterActive: false,
    selectedCompanyDepartmentId: null,
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
            const departmentIdToNodes: DepartmentIdToNode = new Map<string, DepartmentNode>();
            let headDepartment: DepartmentNode = null;

            for (let department of action.departments) {
                const node = new DepartmentNode(
                    department.departmentId,
                    department.parentDepartmentId,
                    department.abbreviation,
                    department.chiefId,
                    null
                );

                departmentIdToNodes.set(node.departmentId, node);

                if (!headDepartment && department.isHeadDepartment) {
                    headDepartment = node;
                }                

                const staffNodeId = `[${node.parentId}-staff]`;

                if (!departmentIdToNodes.has(staffNodeId)) {
                    const staffNode = new DepartmentNode(
                        staffNodeId,
                        node.parentId,
                        null,
                        null,
                        node.parentId
                    );

                    departmentIdToNodes.set(staffNode.departmentId, staffNode);
                }
            }

            appendRoot(headDepartment, departmentIdToNodes);

            return {
                ...state,
                departmentIdToNodes: departmentIdToNodes,
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
            return state;

        case 'ACTIVE-SEARCH-BY-TEXT-FILTER':
            if (action.searchType === SearchType.People) {
                return {
                    ...state,
                    isFilterActive: action.isActive
                };
            }
            return state;

        default:
            return state;
    }
};