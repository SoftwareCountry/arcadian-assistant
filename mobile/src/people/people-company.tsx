import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { View, Text, ScrollView, Dimensions, Animated } from 'react-native';

import { EmployeesList } from './employees-list';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { DepartmentsHScrollableList, DepartmentsListStateDescriptor } from './departments/departments-horizontal-scrollable-list';
import { EmployeeCardWithAvatar } from './employee-card-with-avatar';
import { PeopleActions, updateDepartmentsBranch } from '../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';
import { employeesListStyles as styles } from './styles';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { SearchView, SearchType } from '../navigation/search-view';

interface PeopleCompanySearchProps {
    searchPredicate: (employee: Employee) => boolean;
}

interface PeopleCompanyProps {
    routeName: string;
    departmentsBranch?: Department[];
    departments?: Department[];
    employees: EmployeesStore;
    employee?: Employee;
    currentFocusedDepartmentId?: string;
    departmentLists?: DepartmentsListStateDescriptor[];
    employeesPredicate: (head: Department, employee: Employee) => boolean;
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => ({
        routeName: 'Company',
        departmentsBranch: state.people.departmentsBranch.length > 0 ? state.people.departmentsBranch : null,
        departments: state.people.departments,
        employees: state.organization.employees,
        employee: state.organization.employees.employeesById.get(state.userInfo.employeeId),
        currentFocusedDepartmentId: state.people.currentFocusedDepartmentId,
        departmentLists: state.people.departmentsLists,
        employeesPredicate: (head: Department, employee: Employee) => employee.departmentId === head.departmentId && employee.employeeId !== head.chiefId,
});

interface PeopleCompanyDispatchProps {
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentsBranch: (departmentId: string, focusOnEmployeesList?: boolean) => void;
    onItemClicked: (employee: Employee) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    requestEmployeesForDepartment: (departmentId: string) => { 
        dispatch(loadEmployeesForDepartment(departmentId)); 
    },
    updateDepartmentsBranch: (departmentId: string, focusOnEmployeesList?: boolean) => { 
        dispatch(updateDepartmentsBranch(departmentId, focusOnEmployeesList)); 
    },
    onItemClicked: (employee: Employee) => {
        dispatch( openEmployeeDetailsAction(employee));
    },
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps & PeopleCompanyDispatchProps & PeopleCompanySearchProps> {
    public render() {
        // Branch for current employee
        let userFocusedDepartmentsBranch: Department[] = [];

        if (this.props.departments && this.props.departments.length > 0 && this.props.departmentsBranch !== null) {
                userFocusedDepartmentsBranch = userFocusedDepartmentsBranch.concat(this.props.departmentsBranch);
        }
        let chief = this.props.employees.employeesById.get(userFocusedDepartmentsBranch[0].chiefId);
        let toRender = !userFocusedDepartmentsBranch.every((head) => this.props.employees.employeesById.every((e) => 
            !this.props.searchPredicate(e) || !this.props.employeesPredicate(head, e))) ||
            this.props.searchPredicate(chief);

        return toRender ? 
            <View>
                <ScrollView style={styles.company}>
                    <EmployeeCardWithAvatar
                        employee={chief}
                        departmentAbbreviation={userFocusedDepartmentsBranch[0].abbreviation}
                        treeLevel={0}
                        onItemClicked = {this.props.onItemClicked}
                    />
                    {
                        userFocusedDepartmentsBranch.map((head, index) => {
                            return (
                                <DepartmentsHScrollableList
                                    treeLevel={index + 1}
                                    departments={this.props.departments.filter(department => department.parentDepartmentId === head.departmentId)}
                                    departmentsLists={this.props.departmentLists[index + 1]}
                                    headDepartmentId={head.departmentId}
                                    headDepartmentChiefId={head.chiefId}
                                    focusOnDepartmentWithId={(index + 1) < userFocusedDepartmentsBranch.length ? userFocusedDepartmentsBranch[index + 1].departmentId : null}
                                    currentFocusedDepartmentId={this.props.currentFocusedDepartmentId}
                                    employees={this.props.employees}
                                    key={head.departmentId}
                                    updateDepartmentsBranch={this.props.updateDepartmentsBranch}
                                    requestEmployeesForDepartment={this.props.requestEmployeesForDepartment}
                                    onItemClicked={this.props.onItemClicked}
                                    employeesPredicate={(e) => this.props.searchPredicate(e) && this.props.employeesPredicate(head, e)}
                                />
                            );
                        })
                    }
                </ScrollView>
            </View> : null;
    }
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
