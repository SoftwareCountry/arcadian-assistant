import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { View } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap, EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { employeesAZSort } from './employee-comparer';

interface PeopleDepartmentProps {
    employees: EmployeesStore;
    userEmployee: Employee;
    employeesPredicate: (employee: Employee) => boolean;
}

const mapStateToProps = (state: AppState, ownProps: { employees: EmployeesStore; }): PeopleDepartmentProps => {
    let userEmployee = state.organization.employees.employeesById.get(state.userInfo.employeeId);

    return ({
        employees: ownProps.employees,
        userEmployee,
        employeesPredicate: (employee: Employee) => userEmployee && employee.departmentId === userEmployee.departmentId
    });
};

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee))
});

class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps & EmployeesListDispatchProps> {
    public shouldComponentUpdate(nextProps: PeopleDepartmentProps & EmployeesListDispatchProps) {
        if (this.props.onItemClicked !== nextProps.onItemClicked
            || this.props.userEmployee !== nextProps.userEmployee
        ) {
            return true;
        }

        const employees = this.props.employees.employeesById.filter(this.props.employeesPredicate);
        const nextEmployees = nextProps.employees.employeesById.filter(nextProps.employeesPredicate);

        return !employees.equals(nextEmployees);
    }

    public render() {
        const employees = this.props.employees.employeesById.filter(this.props.employeesPredicate).toArray();

        return <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked} isLoading={this.props.employees.employeesById.size > 0}/>;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);