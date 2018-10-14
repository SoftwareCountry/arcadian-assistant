import React, { Component } from 'react';
import { CompanyDepartmentsLevel } from './company-departments-level';
import { connect, MapStateToProps, MapDispatchToPropsFunction, MapDispatchToProps, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { buildDepartmentIdToNode } from '../reducers/people/build-department-id-to-node';
import { appendRoot, rootId } from '../reducers/people/append-root';
import { filterDepartments } from '../reducers/people/filter-departments';
import { buildBranchFromChildToParent } from '../reducers/people/build-branch-from-child-to-parent';
import { buildDepartmentIdToChildren } from '../reducers/people/build-department-children';
import { Department } from '../reducers/organization/department.model';
import { DepartmentIdToChildren, EmployeeIdToNode, DepartmentIdToSelectedId, MapDepartmentNode, DepartmentNode } from '../reducers/people/people.model';
import { buildEmployeeNodes } from '../reducers/people/build-employee-nodes';
import { buildDepartmentsSelection } from '../reducers/people/build-departments-selection';
import { selectCompanyDepartment, redirectToEmployeeDetails } from '../reducers/people/people.action';
import { buildSelectedDepartmentId } from '../reducers/people/build-selected-department-id';
import { Set } from 'immutable';

interface CompanyDepartmentsStateProps {
    headDepartment: Department;
    departmentIdToChildren: DepartmentIdToChildren;
    employeeIdToNode: EmployeeIdToNode;
    selection: DepartmentIdToSelectedId;
}

interface CompanyDepartmentsDispatchProps {
    selectCompanyDepartment: (departmentId: string) => void;
    onPressEmployee: (employeeId: string) => void;
}

let cachedProps: CompanyDepartmentsStateProps = null;
let cachedDepartmentNodes: Set<MapDepartmentNode>;
let cachedEmployeeNodes: EmployeeIdToNode;
let cachedSelectedCompanyDepartmentId: string;
let cachedFilter: string;

const mapStateToProps: MapStateToProps<CompanyDepartmentsStateProps, void, AppState> = (state: AppState) => {
    if (cachedProps 
        && state.people.departmentNodes.equals(cachedDepartmentNodes) 
        && state.people.employeeNodes.equals(cachedEmployeeNodes)
        && state.people.filter === cachedFilter
        && state.people.selectedCompanyDepartmentId === cachedSelectedCompanyDepartmentId) {
            return cachedProps;
    }

    let departmentsNodes = state.people.departmentNodes.toJS() as DepartmentNode[];
    const departmentIdToNode = buildDepartmentIdToNode(departmentsNodes);

    appendRoot(state.people.headDepartment, departmentIdToNode);

    const employeeIdToNode = buildEmployeeNodes(
        state.people.employeeNodes, 
        state.people.filter);

    let mapDepartmentIdToNode = departmentIdToNode;
    
    if (state.people.filter) {
        departmentsNodes = filterDepartments(departmentsNodes, employeeIdToNode);
        mapDepartmentIdToNode = buildBranchFromChildToParent(departmentsNodes, departmentIdToNode);
    }

    const departmentIdToChildren = buildDepartmentIdToChildren(mapDepartmentIdToNode);

    const selectedCompanyDepartmentId = buildSelectedDepartmentId(mapDepartmentIdToNode, employeeIdToNode, state.people.selectedCompanyDepartmentId);

    const selection = selectedCompanyDepartmentId 
        ? buildDepartmentsSelection(departmentIdToNode, selectedCompanyDepartmentId)
        : {};

    cachedDepartmentNodes = state.people.departmentNodes;
    cachedEmployeeNodes = state.people.employeeNodes; 
    cachedFilter = state.people.filter;
    cachedSelectedCompanyDepartmentId = state.people.selectedCompanyDepartmentId;

    cachedProps = {
        headDepartment: state.people.headDepartment,
        employeeIdToNode: employeeIdToNode,
        departmentIdToChildren: departmentIdToChildren,
        selection: selection
    };

    return cachedProps;
};

const mapDispatchToProps: MapDispatchToProps<CompanyDepartmentsDispatchProps, void> = (dispatch: Dispatch<any>) => ({
    selectCompanyDepartment: (departmentId: string) => {
        dispatch(selectCompanyDepartment(departmentId));
    },
    onPressEmployee: (employeeId: string) => {
        dispatch(redirectToEmployeeDetails(employeeId));
    }
});

type CompanyDepartmentsProps = CompanyDepartmentsStateProps & CompanyDepartmentsDispatchProps;

class CompanyDepartmentsImpl extends Component<CompanyDepartmentsProps> {
    public render() {
        return this.props.headDepartment
            ? <CompanyDepartmentsLevel
                departmentId={rootId}
                departmentIdToChildren={this.props.departmentIdToChildren}
                employeeIdToNode={this.props.employeeIdToNode}
                selection={this.props.selection}
                onSelectedNode={this.props.selectCompanyDepartment}
                onPressEmployee={this.props.onPressEmployee} />
            : null;
    }
}

export const CompanyDepartments = connect(mapStateToProps, mapDispatchToProps)(CompanyDepartmentsImpl);