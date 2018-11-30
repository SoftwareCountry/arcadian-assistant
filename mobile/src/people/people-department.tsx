import React from 'react';
import { connect, MapStateToProps } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { LoadingView } from '../navigation/loading';
import { Action, Dispatch } from 'redux';
import { openEmployeeDetails } from '../navigation/navigation.actions';
import { Optional } from 'types';
import { getEmployee } from '../utils/utils';

interface PeopleDepartmentPropsOwnProps {
    employees: EmployeesStore;
    customEmployeesPredicate?: (employee: Employee) => boolean;
}

interface PeopleDepartmentStateProps {
    employees: EmployeesStore;
    userEmployee: Optional<Employee>;
    employeesPredicate: (employee: Employee) => boolean;
}

type PeopleDepartmentProps = PeopleDepartmentStateProps & PeopleDepartmentPropsOwnProps;

const mapStateToProps: MapStateToProps<PeopleDepartmentProps, PeopleDepartmentPropsOwnProps, AppState> =
        (state: AppState, ownProps: PeopleDepartmentPropsOwnProps): PeopleDepartmentStateProps => {
            const userEmployee = getEmployee(state);
            const defaultEmployeesPredicate = (employee: Employee) => !!userEmployee && employee.departmentId === userEmployee.departmentId;

            return ({
                employees: ownProps.employees,
                userEmployee,
                employeesPredicate: ownProps.customEmployeesPredicate ? ownProps.customEmployeesPredicate : defaultEmployeesPredicate
            });
};

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<Action>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetails(employee))
});

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentStateProps & EmployeesListDispatchProps & PeopleDepartmentPropsOwnProps> {
    public shouldComponentUpdate(nextProps: PeopleDepartmentStateProps & EmployeesListDispatchProps & PeopleDepartmentPropsOwnProps) {
        if (this.props.employees === nextProps.employees
            && this.props.userEmployee === nextProps.userEmployee
            && this.props.employeesPredicate === nextProps.employeesPredicate
            && this.props.onItemClicked === nextProps.onItemClicked) {
                return true;
            }

        const employees = this.props.employees.employeesById.filter(this.props.employeesPredicate);
        const nextEmployees = nextProps.employees.employeesById.filter(nextProps.employeesPredicate);

        return !employees.equals(nextEmployees);
    }

    public render() {
        const employees = this.props.employees.employeesById.toIndexedSeq().toArray().filter(this.props.employeesPredicate);
        if (employees.length === 0) {
            return <LoadingView/>;
        }
        return <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked}/>;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);
