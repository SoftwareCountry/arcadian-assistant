import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { View, Text, ScrollView, Dimensions, Animated } from 'react-native';

import { EmployeesList } from './employees-list';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { DepartmentsHScrollableList } from './departments/departments-horizontal-scrollable-list';
import { DepartmentsTree } from './departments/departments-tree';
import { DepartmentsTreeNode } from './departments/departments-tree-node';
import { EmployeeCardWithAvatar } from './employee-card-with-avatar';
import { PeopleActions, updateDepartmentIdsTree } from '../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';

interface PeopleCompanyProps {
    routeName: string;
    headDepartment: Department;
    departmentsTree: DepartmentsTree;
    departmentsBranch?: DepartmentsTreeNode[];
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => ({
    routeName: 'Company',
    headDepartment: state.people.headDepartment,
    departmentsTree: state.people.departmentsTree,
    departmentsBranch: state.people.departmentsBranch
});

interface PeopleCompanyDispatchProps {
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentIdsTree: (index: number, department: DepartmentsTreeNode) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    requestEmployeesForDepartment: (departmentId: string) => { 
        dispatch(loadEmployeesForDepartment(departmentId)); 
    },
    updateDepartmentIdsTree: (index: number, department: DepartmentsTreeNode) => { 
        dispatch(updateDepartmentIdsTree(index, department)); 
    },
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps & PeopleCompanyDispatchProps> {
    public treeRecurseAndAdd(department: DepartmentsTreeNode, departments: DepartmentsTreeNode[]) {
        departments.push(department);
        var children = department.children;
        if (children !== null) {
            children.forEach(child => this.treeRecurseAndAdd(child, departments));
        }
    }

    public render() {
        const { children } = this.props.departmentsTree.root;

        if (this.props.departmentsBranch === null) {
            return <ScrollView style={{ backgroundColor: '#fff' }} />;
        }

        var heads: DepartmentsTreeNode[] = [];
        var flattenDepartmentsNodes: DepartmentsTreeNode[] = [];
        this.treeRecurseAndAdd(this.props.departmentsTree.root, flattenDepartmentsNodes);

        this.props.departmentsBranch.map((department) => {
            if (department.departmentId === this.props.departmentsTree.root.departmentId) {
                heads.push(this.props.departmentsTree.root);
            } else if (department.departmentId !== 'subordinates') {
                heads.push(flattenDepartmentsNodes.find(departmentNode => departmentNode.departmentId === department.departmentId));
            }
        });

        return <ScrollView style={{ backgroundColor: '#fff', flex: 1 }}>
            <EmployeeCardWithAvatar
                employee={this.props.departmentsTree.root.head}
                departmentAbbreviation={this.props.departmentsTree.root.departmentAbbreviation}
                treeLevel={0}
            />
            {
                heads.map((head) => (
                    <DepartmentsHScrollableList
                        treeLevel={heads.indexOf(head) + 1}
                        departmentsTreeNodes={head.children}
                        headDepartment={head}
                        key={head.departmentId}
                        updateDepartmentIdsTree={this.props.updateDepartmentIdsTree}
                        requestEmployeesForDepartment={this.props.requestEmployeesForDepartment}
                    />
                ))
            }
        </ScrollView>;
    }
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
