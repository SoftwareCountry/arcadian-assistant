import React from 'react';
import { connect, Dispatch } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';

interface PeopleRoomProps {
    employees: EmployeesStore;
    userEmployee: Employee;
    employeesPredicate: (employee: Employee) => boolean;
}

const mapStateToProps = (state: AppState, ownProps: {employees: EmployeesStore}): PeopleRoomProps => {
    const userEmployee = state.organization.employees.employeesById.get(state.userInfo.employeeId);

    return ({
        employees: ownProps.employees,
        userEmployee,
        employeesPredicate: (employee: Employee) => userEmployee && employee.roomNumber === userEmployee.roomNumber
    });
};

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee))
});

class PeopleRoomImpl extends React.Component<PeopleRoomProps & EmployeesListDispatchProps> {
    public shouldComponentUpdate(nextProps: PeopleRoomProps & EmployeesListDispatchProps) {
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
        const employees = this.props.employees.employeesById.toArray().filter(this.props.employeesPredicate);

        return <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked}/>;
    }
}

export const PeopleRoom = connect(mapStateToProps, mapDispatchToProps)(PeopleRoomImpl);