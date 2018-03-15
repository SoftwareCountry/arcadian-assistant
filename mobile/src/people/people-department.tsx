import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';

interface PeopleDepartmentProps {
    employeesMap: EmployeeMap;
    userEmployee: Employee;
}

const mapStateToProps = (state: AppState): PeopleDepartmentProps => ({
    employeesMap: state.organization.employees.employeesById,
    userEmployee: state.userInfo.employee
});
interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch( openEmployeeDetailsAction(employee))
});

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps & EmployeesListDispatchProps> {  
    public render() {
        const predicate = (employee: Employee) => {
            return this.props.userEmployee && employee.departmentId === this.props.userEmployee.departmentId;
        };

        return <EmployeesList employees={this.props.employeesMap.toArray().filter(predicate)} onItemClicked = {this.props.onItemClicked} />;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);