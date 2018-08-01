import React from 'react';
import { connect, Dispatch, MapStateToProps } from 'react-redux';
import { View, ScrollView } from 'react-native';

import { Department } from '../reducers/people/department.model';
import { AppState } from '../reducers/app.reducer';
import { DepartmentsHScrollableList, DepartmentsListStateDescriptor } from './departments/departments-horizontal-scrollable-list';
import { EmployeeCardWithAvatar } from './employee-card-with-avatar';
import { PeopleActions, updateDepartmentsBranch } from '../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { Employee } from '../reducers/organization/employee.model';
import { employeesListStyles as styles } from './styles';
import { EmployeesStore } from '../reducers/organization/employees.reducer';

interface PeopleCompanySearchOwnProps {
    employees: EmployeesStore;
}

interface PeopleCompanyStateProps {
    routeName: string;
    departmentsBranch: Department[];
    departments: Department[];
    employee: Employee;
    currentFocusedDepartmentId: string;
    departmentLists: DepartmentsListStateDescriptor[];
    employeesPredicate: (head: Department, employee: Employee) => boolean;
}

const mapStateToProps: MapStateToProps<PeopleCompanySearchProps, PeopleCompanySearchOwnProps, AppState> = (state: AppState, ownProps): PeopleCompanySearchProps => ({
        employees: ownProps.employees, // own props

        routeName: 'Company', 
        departmentsBranch: state.people.departmentsBranch.length > 0 ? state.people.departmentsBranch : null,
        departments: state.people.departments,
        employee: state.organization.employees.employeesById.get(state.userInfo.employeeId),
        currentFocusedDepartmentId: state.people.currentFocusedDepartmentId,
        departmentLists: state.people.departmentsLists,
        employeesPredicate: (head: Department, employee: Employee) => employee.departmentId === head.departmentId && employee.employeeId !== head.chiefId,
});

type PeopleCompanySearchProps = PeopleCompanySearchOwnProps & PeopleCompanyStateProps;

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

export class PeopleCompanyImpl extends React.Component<PeopleCompanySearchProps & PeopleCompanyDispatchProps> {
    public render() {
        // Branch for current employee
        let userFocusedDepartmentsBranch: Department[] = [];

        if (this.props.departments && this.props.departments.length > 0 && this.props.departmentsBranch !== null) {
                userFocusedDepartmentsBranch = userFocusedDepartmentsBranch.concat(this.props.departmentsBranch);
        }
        const chief = this.props.employees.employeesById.get(userFocusedDepartmentsBranch[0].chiefId);

        return <View>
                <ScrollView style={styles.company}>
                    <EmployeeCardWithAvatar
                        employee={chief}
                        departmentAbbreviation={userFocusedDepartmentsBranch[0].abbreviation}
                        treeLevel={0}
                        onItemClicked = {chief ? this.props.onItemClicked : () => {}}
                    />
                    {
                        userFocusedDepartmentsBranch.map((head, index) => {
                            const itemClicked = (e: Employee) => { if (e) { this.props.onItemClicked(e); }};
                            const employeesPredicate = (e: Employee) => this.props.employeesPredicate(head, e);

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
                                    onItemClicked={itemClicked}
                                    employeesPredicate={employeesPredicate}
                                />
                            );
                        })
                    }
                </ScrollView>
            </View>;
    }
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
