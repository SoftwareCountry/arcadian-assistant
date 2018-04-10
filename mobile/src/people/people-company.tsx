import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { View, Text, ScrollView, Dimensions, Animated } from 'react-native';

import { EmployeesList } from './employees-list';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { DepartmentsHScrollableList } from './departments/departments-horizontal-scrollable-list';
import { DepartmentsTree } from './departments/departments-tree';
import { DepartmentsTreeNode, stubIdForSubordinates } from './departments/departments-tree-node';
import { EmployeeCardWithAvatar } from './employee-card-with-avatar';
import { PeopleActions, updateDepartmentIdsTree } from '../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';
import { employeesListStyles as styles } from './styles';
import { EmployeesStore } from '../reducers/organization/employees.reducer';

interface PeopleCompanyProps {
    routeName: string;
    headDepartment: Department;
    departmentsTree: DepartmentsTree;
    departmentsBranch?: DepartmentsTreeNode[];
    employees: EmployeesStore;
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => ({
    routeName: 'Company',
    headDepartment: state.people.headDepartment,
    departmentsTree: state.people.departmentsTree,
    departmentsBranch: state.people.departmentsBranch,
    employees: state.organization.employees
});

interface PeopleCompanyDispatchProps {
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentIdsTree: (index: number, department: DepartmentsTreeNode) => void;
    onItemClicked: (employee: Employee) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    requestEmployeesForDepartment: (departmentId: string) => { 
        dispatch(loadEmployeesForDepartment(departmentId)); 
    },
    updateDepartmentIdsTree: (index: number, department: DepartmentsTreeNode) => { 
        dispatch(updateDepartmentIdsTree(index, department)); 
    },
    onItemClicked: (employee: Employee) => {
        dispatch( openEmployeeDetailsAction(employee));
    },
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps & PeopleCompanyDispatchProps> {
    public treeRecurseAndAdd(department: DepartmentsTreeNode, departments: DepartmentsTreeNode[]) {
        departments.push(department);
        const children = department.children;
        if (children !== null) {
            children.forEach(child => this.treeRecurseAndAdd(child, departments));
        }
    }

    public render() {
        if (this.props.departmentsBranch === null || this.props.departmentsTree === null) {
            return <View style={styles.loadingContainer}>
                        <StyledText style={styles.loadingText}>Loading...</StyledText>
                    </View>;
        }

        const { children } = this.props.departmentsTree.root;

        const heads: DepartmentsTreeNode[] = [];
        const flattenDepartmentsNodes: DepartmentsTreeNode[] = [];
        this.treeRecurseAndAdd(this.props.departmentsTree.root, flattenDepartmentsNodes);

        for (const department of this.props.departmentsBranch) {
            if (department.departmentId === this.props.departmentsTree.root.departmentId) {
                heads.push(this.props.departmentsTree.root);
            } else if (department.departmentId !== stubIdForSubordinates) {
                heads.push(flattenDepartmentsNodes.find(departmentNode => departmentNode.departmentId === department.departmentId));
            }
        }

        return <ScrollView style={{ backgroundColor: '#fff', flex: 1 }}>
            <EmployeeCardWithAvatar
                employee={this.props.employees.employeesById.get(this.props.departmentsTree.root.departmentChiefId)}
                departmentAbbreviation={this.props.departmentsTree.root.departmentAbbreviation}
                treeLevel={0}
                onItemClicked = {this.props.onItemClicked}
            />
            {
                heads.map((head) => (
                    <DepartmentsHScrollableList
                        treeLevel={heads.indexOf(head) + 1}
                        departmentsTreeNodes={head.children}
                        headDepartment={head}
                        employees={this.props.employees}
                        key={head.departmentId}
                        updateDepartmentIdsTree={this.props.updateDepartmentIdsTree}
                        requestEmployeesForDepartment={this.props.requestEmployeesForDepartment}
                        onItemClicked={this.props.onItemClicked}
                        employeesPredicate={(employee: Employee) => { 
                            return employee.departmentId === head.departmentId;
                        }}
                    />
                ))
            }
        </ScrollView>;
    }
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
