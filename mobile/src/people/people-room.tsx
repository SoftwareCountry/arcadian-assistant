import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';

interface PeopleRoomProps {
    employeesMap: EmployeeMap;
    userEmployee: Employee;
}

const mapStateToProps = (state: AppState): PeopleRoomProps => ({
    employeesMap: state.organization.employees.employeesById,
    userEmployee: state.userInfo.employee
});

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch( openEmployeeDetailsAction(employee))
});

export class PeopleRoomImpl extends React.Component<PeopleRoomProps & EmployeesListDispatchProps> {  
    public render() {
        const predicate = (employee: Employee) => {
            return this.props.userEmployee && employee.roomNumber === this.props.userEmployee.roomNumber;
        };

        return <EmployeesList employees={this.props.employeesMap.toArray().filter(predicate)} onItemClicked = {this.props.onItemClicked}/>;
    }
}

export const PeopleRoom = connect(mapStateToProps, mapDispatchToProps)(PeopleRoomImpl);