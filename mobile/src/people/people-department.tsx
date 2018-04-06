import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap, EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';

interface PeopleDepartmentProps {
    employees: EmployeesStore;
    userEmployee: Employee;
}

const mapStateToProps = (state: AppState): PeopleDepartmentProps => ({
    employees: state.organization.employees,
    userEmployee: state.userInfo.employee
});
interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch( openEmployeeDetailsAction(employee))
});

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps & EmployeesListDispatchProps> {
    public shouldComponentUpdate(nextProps: PeopleDepartmentProps & EmployeesListDispatchProps) {
        if (this.props.onItemClicked !== nextProps.onItemClicked
            || this.props.userEmployee !== nextProps.userEmployee
        ) {
            return true;
        }

        const predicate = (employee: Employee) => {
            return this.props.userEmployee && employee.departmentId === this.props.userEmployee.departmentId;
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
            return this.props.userEmployee && employee.departmentId === this.props.userEmployee.departmentId;
        };

        return <EmployeesList employees={this.props.employees.employeesById.toArray().filter(predicate)} onItemClicked = {this.props.onItemClicked} />;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);