import React from 'react';
import { connect, Dispatch, MapStateToProps } from 'react-redux';
import { View, ScrollView } from 'react-native';

import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { DepartmentsHScrollableList, DepartmentsListStateDescriptor } from './departments/departments-horizontal-scrollable-list';
import { EmployeeCardWithAvatar } from './departments/employee-card-with-avatar';
import { EmployeeCardWithAvatarList } from './departments/employee-card-with-avatar-list';
import { PeopleActions, updateDepartmentsBranch } from '../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { Employee } from '../reducers/organization/employee.model';
import { employeesListStyles as styles } from './styles';
import { EmployeesStore } from '../reducers/organization/employees.reducer';

interface PeopleCompanySearchOwnProps {
    employees: EmployeesStore;
    departments: Department[];
}

interface PeopleCompanyStateProps {
    routeName: string;
    departmentsBranch: Department[];
    employee: Employee;
    currentFocusedDepartmentId: string;
    departmentLists: DepartmentsListStateDescriptor[];
    employeesPredicate: (head: Department, employee: Employee) => boolean;
}

const mapStateToProps: MapStateToProps<PeopleCompanySearchProps, PeopleCompanySearchOwnProps, AppState> = (state: AppState, ownProps): PeopleCompanySearchProps => ({
        employees: ownProps.employees, // own props
        departments: ownProps.departments,

        routeName: 'Company', 
        departmentsBranch: state.people.departmentsBranch,
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
        if (employee) {
            dispatch(openEmployeeDetailsAction(employee));
        }
    },
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanySearchProps & PeopleCompanyDispatchProps> {
    public render() {
        const chief = this.props.employees.employeesById.get(this.props.departmentsBranch[0].chiefId);

        //list of employees
        const lowestLevel = this.props.departmentsBranch[this.props.departmentsBranch.length - 1];
        const lowestLevelDesc = this.props.departmentLists[this.props.departmentsBranch.length - 1];
        const employeesPredicate = (e: Employee) => this.props.employeesPredicate(lowestLevel, e);
        let subordinates;
        if (this.props.employees.employeeIdsByDepartment.has(lowestLevel.departmentId) && 
            this.props.employees.employeeIdsByDepartment.get(lowestLevel.departmentId).size > 0) {
            subordinates = this.props.employees.employeesById.filter(employeesPredicate).toArray();
        } else {
            subordinates = null;
        }

        return <View>
                <ScrollView style={styles.company}>
                    <EmployeeCardWithAvatar
                        employee={chief}
                        departmentAbbreviation={this.props.departmentsBranch[0].abbreviation}
                        onItemClicked = {this.props.onItemClicked}
                    />
                    {
                        this.props.departmentsBranch.map((head, index) => {
                            return (
                                <DepartmentsHScrollableList
                                    departments={this.props.departments.filter(department => department.parentDepartmentId === head.departmentId)}
                                    departmentsLists={this.props.departmentLists[index + 1]}
                                    headDepartment={head}
                                    employees={this.props.employees}
                                    key={head.departmentId}
                                    updateDepartmentsBranch={this.props.updateDepartmentsBranch}
                                    requestEmployeesForDepartment={this.props.requestEmployeesForDepartment}
                                    onItemClicked={this.props.onItemClicked}
                                />
                            );
                        })
                    }
                    {
                    (subordinates != null && subordinates.length > 0) ? 
                        <EmployeeCardWithAvatarList 
                            employees={subordinates.sort()} 
                            chiefId={lowestLevel.chiefId}
                            treeLevel={this.props.departmentsBranch.length - 1}
                            onItemClicked={this.props.onItemClicked}
                        /> 
                        : null
                    }
                </ScrollView>
            </View>;
    }
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
