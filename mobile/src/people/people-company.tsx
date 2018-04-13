import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { View, Text, ScrollView, Dimensions, Animated } from 'react-native';

import { EmployeesList } from './employees-list';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { DepartmentsHScrollableList } from './departments/departments-horizontal-scrollable-list';
import { EmployeeCardWithAvatar } from './employee-card-with-avatar';
import { PeopleActions, updateDepartmentsBranch } from '../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';
import { employeesListStyles as styles } from './styles';
import { EmployeesStore } from '../reducers/organization/employees.reducer';

interface PeopleCompanyProps {
    routeName: string;
    departmentsBranch?: Department[];
    employees: EmployeesStore;
    departments?: Department[];
    employee?: Employee;
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => ({
    routeName: 'Company',
    departmentsBranch: state.people.departmentsBranch.length > 0 ? state.people.departmentsBranch : null,
    employees: state.organization.employees,
    departments: state.people.departments,
    employee: state.userInfo.employee
});

interface PeopleCompanyDispatchProps {
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentsBranch: (departmentId: string) => void;
    onItemClicked: (employee: Employee) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    requestEmployeesForDepartment: (departmentId: string) => { 
        dispatch(loadEmployeesForDepartment(departmentId)); 
    },
    updateDepartmentsBranch: (departmentId: string) => { 
        dispatch(updateDepartmentsBranch(departmentId)); 
    },
    onItemClicked: (employee: Employee) => {
        dispatch( openEmployeeDetailsAction(employee));
    },
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps & PeopleCompanyDispatchProps> {
    public render() {
        // Branch for current employee
        let userFocusedDepartmentsBranch: Department[] = [];

        if (this.props.departments && this.props.departments.length > 0 && this.props.departmentsBranch !== null) {
                userFocusedDepartmentsBranch = userFocusedDepartmentsBranch.concat(this.props.departmentsBranch);
        } else {
            return <View style={styles.loadingContainer}>
                        <StyledText style={styles.loadingText}>Loading...</StyledText>
                    </View>;
        }
        
        return <ScrollView style={{ backgroundColor: '#fff', flex: 1 }}>
            <EmployeeCardWithAvatar
                employee={this.props.employees.employeesById.get(userFocusedDepartmentsBranch[0].chiefId)}
                departmentAbbreviation={userFocusedDepartmentsBranch[0].abbreviation}
                treeLevel={0}
                onItemClicked = {this.props.onItemClicked}
            />
            {
                userFocusedDepartmentsBranch.map((head, index) => (
                    <DepartmentsHScrollableList
                        treeLevel={index + 1}
                        departments={this.props.departments.filter(department => department.parentDepartmentId === head.departmentId)}
                        headDepartmentId={head.departmentId}
                        headDepartmentChiefId={head.chiefId}
                        focusOnDepartmentWithId={(index + 1) < userFocusedDepartmentsBranch.length ? userFocusedDepartmentsBranch[index + 1].departmentId : null}
                        employees={this.props.employees}
                        key={head.departmentId}
                        updateDepartmentsBranch={this.props.updateDepartmentsBranch}
                        requestEmployeesForDepartment={this.props.requestEmployeesForDepartment}
                        onItemClicked={this.props.onItemClicked}
                        employeesPredicate={(employee: Employee) => { 
                            return employee.departmentId === head.departmentId && employee.employeeId !== head.chiefId;
                        }}
                    />
                ))
            }
        </ScrollView>;
    }
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
