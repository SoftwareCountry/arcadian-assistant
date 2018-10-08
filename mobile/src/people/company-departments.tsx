import React, { Component } from 'react';
import { CompanyDepartmentsLevel } from './company-departments-level';
import { connect, MapStateToProps, MapDispatchToPropsFunction, MapDispatchToProps, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { buildDepartmentIdToNode } from '../reducers/people/build-department-id-to-node';
import { appendRoot, rootId } from '../reducers/people/append-root';
import { filterDepartments } from '../reducers/people/filter-departments';
import { buildBranchFromChildToParent } from '../reducers/people/build-branch-from-child-to-parent';
import { buildDepartmentIdToChildren } from '../reducers/people/build-department-children';
import { Department } from '../reducers/organization/department.model';
import { DepartmentIdToChildren, EmployeeIdToNode, DepartmentIdToSelectedId } from '../reducers/people/people.model';
import { buildEmployeeNodes } from '../reducers/people/build-employee-nodes';
import { buildDepartmentsSelection } from '../reducers/people/build-departments-selection';
import { selectCompanyDepartment } from '../reducers/people/people.action';

interface CompanyDepartmentsStateProps {
    headDepartment: Department;
    employeeIdsByDepartment: EmployeeIdsGroupMap;
    departmentIdToChildren: DepartmentIdToChildren;
    employeeIdToNode: EmployeeIdToNode;
    selection: DepartmentIdToSelectedId;
}

interface CompanyDepartmentsDispatchProps {
    selectCompanyDepartment: (departmentId: string, allowSelect: boolean) => void;
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

    const userEmployee = state.organization.employees.employeesById.get(state.userInfo.employeeId);

    const selection = userEmployee 
        ? buildDepartmentsSelection(departmentIdToNode, state.people.selectedCompanyDepartmentId, state.people.allowSelect)
        : {};

    return {
        headDepartment: headDepartment,
        employeeIdsByDepartment: state.organization.employees.employeeIdsByDepartment,
        employeeIdToNode: employeeIdToNode,
        departmentIdToChildren: departmentIdToChildren,
        selection: selection
    };
};

const mapDispatchToProps: MapDispatchToProps<CompanyDepartmentsDispatchProps, void> = (dispatch: Dispatch<any>) => ({
    selectCompanyDepartment: (departmentId: string, allowSelect: boolean) => {
        dispatch(selectCompanyDepartment(departmentId, allowSelect));
    }
});

type CompanyDepartmentsProps = CompanyDepartmentsStateProps & CompanyDepartmentsDispatchProps;

class CompanyDepartmentsImpl extends Component<CompanyDepartmentsProps> {
    public render() {
        return this.props.headDepartment
            ? <CompanyDepartmentsLevel
                departmentId={rootId}
                departmentIdToChildren={this.props.departmentIdToChildren}
                employeeIdsByDepartment={this.props.employeeIdsByDepartment}
                employeeIdToNode={this.props.employeeIdToNode}
                selection={this.props.selection}
                onSelectedNode={this.props.selectCompanyDepartment} />
            : null;
    }
}

export const CompanyDepartments = connect(mapStateToProps, mapDispatchToProps)(CompanyDepartmentsImpl);