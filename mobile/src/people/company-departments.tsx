import React, { Component } from 'react';
import { View } from 'react-native';
import { companyDepartments } from './styles';
import { layout } from '../calendar/event-dialog/styles';
import { CompanyDepartmentsLevel } from './company-departments-level';
import { connect, MapStateToProps } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { buildDepartmentIdToNode } from '../reducers/people/build-department-id-to-node';
import { appendRoot, rootId } from '../reducers/people/append-root';
import { filterDepartments } from '../reducers/people/filter-departments';
import { buildBranchFromChildToParent } from '../reducers/people/build-branch-from-child-to-parent';
import { buildDepartmentIdToChildren } from '../reducers/people/build-department-children';
import { Department } from '../reducers/organization/department.model';
import { DepartmentIdToChildren, EmployeeIdToNode } from '../reducers/people/people.model';
import { buildEmployeeNodes } from '../reducers/people/build-employee-nodes';

interface CompanyDepartmentsStateProps {
    headDepartment: Department;
    employeeIdsByDepartment: EmployeeIdsGroupMap;
    departmentIdToChildren: DepartmentIdToChildren;
    employeeIdToNode: EmployeeIdToNode;
}

interface CompanyDepartmentsDispatchProps {

}

const mapStateToProps: MapStateToProps<CompanyDepartmentsStateProps, void, AppState> = (state: AppState) => {
    const departmentIdToNode = buildDepartmentIdToNode(state.organization.departments);
    const headDepartment = state.organization.departments.find(department => department.isHeadDepartment);

    appendRoot(headDepartment, departmentIdToNode);

    const filteredDepartments = filterDepartments(
        null,
        state.organization.departments,
        state.organization.employees.employeesById,
        state.organization.employees.employeeIdsByDepartment);

    const withBranches = buildBranchFromChildToParent(filteredDepartments, departmentIdToNode);

    const departmentIdToChildren = buildDepartmentIdToChildren(withBranches);

    const employeeIdToNode = buildEmployeeNodes(state.organization.employees.employeesById);

    return {
        headDepartment: headDepartment,
        employeeIdsByDepartment: state.organization.employees.employeeIdsByDepartment,
        employeeIdToNode: employeeIdToNode,
        departmentIdToChildren: departmentIdToChildren
    };
};

type CompanyDepartmentsProps = CompanyDepartmentsStateProps & CompanyDepartmentsDispatchProps;

class CompanyDepartmentsImpl extends Component<CompanyDepartmentsProps> {
    public render() {
        return this.props.headDepartment
            ? <CompanyDepartmentsLevel
                departmentId={rootId}
                departmentIdToChildren={this.props.departmentIdToChildren}
                employeeIdsByDepartment={this.props.employeeIdsByDepartment}
                employeeIdToNode={this.props.employeeIdToNode} />
            : null;
    }
}

export const CompanyDepartments = connect(mapStateToProps)(CompanyDepartmentsImpl);