import { Reducer } from 'redux';
import { NavigationAction } from 'react-navigation';
import { LoadDepartmentsFinished, LoadEmployeeFinished } from '../organization/organization.action';
import { PeopleActions } from './people.action';
import { SearchActions } from '../search/search.action';
import { SearchType } from '../../navigation/search-view';
import { Department } from '../organization/department.model';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { combineEpics } from 'redux-observable';
import { companyDepartmentSelected$, redirectToEmployeeDetails$ } from './people.epics';
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
            
            const departmentNodes = action.departments.map(x => {
                const node: DepartmentNode = {
                    departmentId: x.departmentId,
                    parentId: x.parentDepartmentId,
                    abbreviation: x.abbreviation,
                    chiefId: x.chiefId
                };             
                return Map<keyof DepartmentNode, DepartmentNode[keyof DepartmentNode]>(node);
            });

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
    companyDepartmentSelected$ as any,
    redirectToEmployeeDetails$ as any
);