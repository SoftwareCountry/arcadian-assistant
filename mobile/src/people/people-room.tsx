import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';

interface PeopleRoomProps {
    employeesMap: EmployeeMap;
    userEmployee: Employee;
}

const mapStateToProps = (state: AppState): PeopleRoomProps => ({
    employeesMap: state.organization.employees.employeesById,
    userEmployee: state.userInfo.employee
});

export class PeopleRoomImpl extends React.Component<PeopleRoomProps> {  
    public render() {
        const predicate = (employee: Employee) => {
            return this.props.userEmployee && employee.roomNumber === this.props.userEmployee.roomNumber;
        };

        return <EmployeesList employees={this.props.employeesMap.toArray().filter(predicate)} />;
    }
}

export const PeopleRoom = connect(mapStateToProps)(PeopleRoomImpl);