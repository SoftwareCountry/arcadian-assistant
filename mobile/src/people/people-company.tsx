import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
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
import { EmployeeCardWithAvatar } from './employee-card-with-avatar';
import { PeopleActions, updateDepartmentIdsTree } from '../reducers/people/people.action';

interface PeopleCompanyProps {
    routeName: string;
    headDepartment: Department;
    departmentsTree: DepartmentsTree;
    departmentIdsBranch?: string[];
}

interface PeopleCompanyDispatchProps {
    updateDepartmentIdsTree: (index: number, departmentId: string) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    updateDepartmentIdsTree: (index: number, departmentId: string) => {
        dispatch(updateDepartmentIdsTree(index, departmentId));
    },
});

const mapStateToProps = (state: AppState): PeopleCompanyProps => ({
    routeName: 'Company',
    headDepartment: state.organization.departments.filter((department) => department.isHeadDepartment === true)[0],
    departmentsTree: departmentsTreeFor(state.organization),
    departmentIdsBranch: state.people.departmentIdsBranch
});

function departmentsTreeFor(organization: OrganizationState) {
    const topLevelDepartment: Department = organization.departments.filter((department) => department.isHeadDepartment === true)[0];
    const topLevelEmployee: Employee = organization.employees.employeesById.get(topLevelDepartment.chiefId);
    const departmentsTree: DepartmentsTree = {
        root: {
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
            subordinates: employees.filter((emp) => emp.departmentId === department.departmentId).toArray()
        });
    });

    return nodes;
}

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps & PeopleCompanyDispatchProps> {
    public componentWillMount() {
        const newDepartmentIdsBranch = [];
        var currentDepartmentNode = this.props.departmentsTree.root;
        var index = 0;

        while (currentDepartmentNode) {
            console.log(currentDepartmentNode.departmentAbbreviation + '/' + currentDepartmentNode.departmentId);
            newDepartmentIdsBranch.push(currentDepartmentNode.departmentId);
            this.props.updateDepartmentIdsTree(index++, currentDepartmentNode.departmentId);
            const firstChild = currentDepartmentNode.children != null ? currentDepartmentNode.children[0] : null;
            currentDepartmentNode = firstChild;
        }
        // console.log(newDepartmentIdsBranch);
    }

    public treeRecurseAndAdd(department: DepartmentsTreeNode, departments: DepartmentsTreeNode[]) {
        departments.push(department);
        var children = department.children;
        if (children !== null) {
            children.forEach(child => this.treeRecurseAndAdd(child, departments));
        }
    }

    public render() {
        const { children } = this.props.departmentsTree.root;

        if (this.props.departmentIdsBranch === null) {
            return <ScrollView style={{ backgroundColor: '#fff' }} />;
        }

        var heads: DepartmentsTreeNode[] = [];
        var flattenDepartmentsNodes: DepartmentsTreeNode[] = [];
        this.treeRecurseAndAdd(this.props.departmentsTree.root, flattenDepartmentsNodes);

        this.props.departmentIdsBranch.map((departmentId) => {
            if (departmentId === this.props.departmentsTree.root.departmentId) {
                heads.push(this.props.departmentsTree.root);
            } else if (departmentId !== 'subordinates') {
                heads.push(flattenDepartmentsNodes.filter(departmentNode => departmentNode.departmentId === departmentId)[0]);
            }
        });

        return <ScrollView style={{ backgroundColor: '#fff' }}>
            <EmployeeCardWithAvatar
                employee={this.props.departmentsTree.root.head}
                departmentAbbreviation={this.props.departmentsTree.root.departmentAbbreviation}
                treeLevel={0}
            />
            {
                heads.map((head) => (
                    <DepartmentsHScrollableList
                        departmentsTree={this.props.departmentsTree}
                        treeLevel={heads.indexOf(head) + 1}
                        departmentsTreeNodes={head.children}
                        headDepartment={head}
                        key={head.departmentId}
                    />
                ))
            }
        </ScrollView>;
    }
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
