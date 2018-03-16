import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { View, Text, ScrollView, Dimensions, Animated } from 'react-native';

import { EmployeesList } from './employees-list';
import { OrganizationState } from '../reducers/organization/organization.reducer';
import { Department } from '../reducers/organization/department.model';
import { Employee } from '../reducers/organization/employee.model';
import { AppState } from '../reducers/app.reducer';
import { DepartmentsHScrollableList } from './departments/departments-horizontal-scrollable-list';
import { DepartmentsTree } from './departments/departments-tree';
import { DepartmentsTreeNode } from './departments/departments-tree-node';
import { EmployeeMap, EmployeesStore } from '../reducers/organization/employees.reducer';

interface PeopleCompanyProps {
    routeName: string;
    headDepartment: Department;
    departmentsTree: DepartmentsTree;
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => ({
    routeName: 'Company',
    headDepartment: state.organization.departments.filter((department) => department.isHeadDepartment === true)[0],
    departmentsTree: departmentsTreeFor(state.organization)
});

function departmentsTreeFor(organization: OrganizationState) {
    const topLevelDepartment: Department = organization.departments.filter((department) => department.isHeadDepartment === true)[0];
    const topLevelEmployee: Employee = organization.employees.employeesById.get(topLevelDepartment.chiefId);
    const departmentsTree: DepartmentsTree = { root: { 
            departmentId: topLevelDepartment.departmentId, 
            departmentAbbreviation: topLevelDepartment.abbreviation,
            departmentChiefId: topLevelDepartment.chiefId,
            head: topLevelEmployee, 
            parent: null, 
            children: childrenNodes(topLevelDepartment, organization.departments, organization.employees.employeesById),
            subordinates: organization.employees.employeesById.filter((employee) => employee.departmentId === topLevelDepartment.departmentId).toArray()
        } 
    };

    return departmentsTree;
}

function childrenNodes(headDeaprtment: Department, departments: Department[], employees: EmployeeMap) {
    var nodes: DepartmentsTreeNode[] = [];
    const sublings = departments.filter((subDepartment) => subDepartment.parentDepartmentId === headDeaprtment.departmentId);
    sublings.forEach(department => {
        const employee: Employee = employees.get(department.chiefId);
        const subSublings = departments.filter((subDepartment) => subDepartment.parentDepartmentId === department.departmentId);
        nodes.push({ 
            departmentId: department.departmentId, 
            departmentAbbreviation: department.abbreviation,
            departmentChiefId: department.chiefId,
            head: employee, 
            parent: headDeaprtment, 
            children: subSublings.length > 0 ? childrenNodes(department, departments, employees) : null, 
            subordinates: employees.filter((emp) => emp.departmentId === headDeaprtment.departmentId).toArray() });
    });

    return nodes;
}

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps> {
    public render() {
        console.log('Head department: ' + this.props.headDepartment.name);
        // this.props.departmentsTree.root.children.values('head')
        console.log(this.props.departmentsTree.root.children.map(a => a.head));

        return <ScrollView style={{ backgroundColor: '#fff' }}>
            <DepartmentsHScrollableList departmentsTree={this.props.departmentsTree} employees={[this.props.departmentsTree.root.head]} />
            <DepartmentsHScrollableList departmentsTree={this.props.departmentsTree} employees={this.props.departmentsTree.root.children.map(a => a.head)} subordinates={this.props.departmentsTree.root.subordinates.length > 0 ? this.props.departmentsTree.root.subordinates : null} />
            <DepartmentsHScrollableList departmentsTree={this.props.departmentsTree} employees={[]} />
        </ScrollView>;
    }
}

export const PeopleCompany = connect(mapStateToProps)(PeopleCompanyImpl);
