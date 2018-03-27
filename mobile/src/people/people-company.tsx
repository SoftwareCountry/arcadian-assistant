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

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps> {
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
            console.log('TREE head ' + department.departmentId);
            if (department.departmentId === this.props.departmentsTree.root.departmentId) {
                heads.push(this.props.departmentsTree.root);
            } else if (department.departmentId !== 'subordinates') {
                heads.push(flattenDepartmentsNodes.filter(departmentNode => departmentNode.departmentId === department.departmentId)[0]);
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
                    />
                ))
            }
        </ScrollView>;
    }
}

export const PeopleCompany = connect(mapStateToProps)(PeopleCompanyImpl);
