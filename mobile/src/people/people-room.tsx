import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap, EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';

interface PeopleRoomProps {
    employees: EmployeesStore;
    userEmployee: Employee;
}

const mapStateToProps = (state: AppState): PeopleRoomProps => ({
    employees: state.organization.employees,
    userEmployee: state.userInfo.employee
});

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch( openEmployeeDetailsAction(employee))
});

export class PeopleRoomImpl extends React.Component<PeopleRoomProps & EmployeesListDispatchProps> {
    public shouldComponentUpdate(nextProps: PeopleRoomProps & EmployeesListDispatchProps) {
        if (this.props.onItemClicked !== nextProps.onItemClicked
            || this.props.userEmployee !== nextProps.userEmployee
        ) {
            return true;
        }

        const predicate = (employee: Employee) => {
            return this.props.userEmployee && employee.roomNumber === this.props.userEmployee.roomNumber;
        };

        const nextEmployees = nextProps.employees.employeesById.toArray().filter(predicate);

        for (const employee of nextEmployees) {
            const employeeId = employee.employeeId;
            const currentEmployeeRef = this.props.employees.employeesById.get(employeeId);
            const nextEmployeeRef = nextProps.employees.employeesById.get(employeeId);            

            if (currentEmployeeRef !== nextEmployeeRef) {
                return true;
            }
        }

        return false;
    }

    public render() {
        const predicate = (employee: Employee) => {
            return this.props.userEmployee && employee.roomNumber === this.props.userEmployee.roomNumber;
        };

        return <EmployeesList employees={this.props.employees.employeesById.toArray().filter(predicate)} onItemClicked = {this.props.onItemClicked}/>;
    }
}

export const PeopleRoom = connect(mapStateToProps, mapDispatchToProps)(PeopleRoomImpl);