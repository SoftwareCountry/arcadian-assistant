import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';

interface PeopleDepartmentProps {
    employeesMap: EmployeeMap;
    userEmployee: Employee;
}

const mapStateToProps = (state: AppState): PeopleDepartmentProps => ({
    employeesMap: state.organization.employees.employeesById,
    userEmployee: state.userInfo.employee
});

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps> {  
    public render() {
        const predicate = function(employee: Employee) {
            return this.props.userEmployee && employee.departmentId === this.props.userEmployee.departmentId;
        };

        return <EmployeesList employees={this.props.employeesMap.toArray().filter(predicate)} />;
    }
}

export const PeopleDepartment = connect(mapStateToProps)(PeopleDepartmentImpl);