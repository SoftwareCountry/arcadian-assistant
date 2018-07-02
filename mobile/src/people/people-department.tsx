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
    filter: string;
    employeesPredicate: (employee: Employee) => boolean;
}

const mapStateToProps = (state: AppState): PeopleDepartmentProps => {
    const filter = state.people.filter;
    const userEmployee = state.organization.employees.employeesById.get(state.userInfo.employeeId);

    return ({
        employees: state.organization.employees,
        userEmployee,
        filter,
        employeesPredicate: (employee: Employee) => (employee.name.includes(filter) ||
                                                    employee.email.includes(filter) || 
                                                    employee.position.includes(filter)) &&
                                                    userEmployee && employee.departmentId === userEmployee.departmentId,
    });
};

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee))
});

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps & EmployeesListDispatchProps> {
    public shouldComponentUpdate(nextProps: PeopleDepartmentProps & EmployeesListDispatchProps) {
        if (this.props.onItemClicked !== nextProps.onItemClicked
            || this.props.userEmployee !== nextProps.userEmployee
        ) {
            return true;
        }

        const employees = this.props.employees.employeesById.filter(this.props.employeesPredicate);
        const nextEmployees = nextProps.employees.employeesById.filter(this.props.employeesPredicate);

        if (!employees.equals(nextEmployees)) {
            return true;
        } else {
            return false;
        }
    }

    public render() {
        const employees = this.props.employees.employeesById.toArray().filter(this.props.employeesPredicate);

        return <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked} />;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);