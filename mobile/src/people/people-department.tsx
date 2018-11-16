import React, {createRef} from 'react';
import {connect, connectAdvanced, Dispatch, MapStateToProps} from 'react-redux';

import {EmployeesList} from './employees-list';
import {AppState} from '../reducers/app.reducer';
import {EmployeesStore} from '../reducers/organization/employees.reducer';
import {Employee} from '../reducers/organization/employee.model';
import {openEmployeeDetailsAction} from '../employee-details/employee-details-dispatcher';
import {NavigationScreenProps} from 'react-navigation';

interface PeopleDepartmentPropsOwnProps {
    employees: EmployeesStore;
    customEmployeesPredicate?: (employee: Employee) => boolean;
}

interface PeopleDepartmentStateProps {
    employees: EmployeesStore;
    userEmployee: Employee;
    employeesPredicate: (employee: Employee) => boolean;
}

type PeopleDepartmentProps = PeopleDepartmentStateProps & PeopleDepartmentPropsOwnProps;

const mapStateToProps: MapStateToProps<PeopleDepartmentProps, PeopleDepartmentPropsOwnProps, AppState> =
    (state: AppState, ownProps: PeopleDepartmentPropsOwnProps): PeopleDepartmentStateProps => {
        const userEmployee = state.organization.employees.employeesById.get(state.userInfo.employeeId);
        const defaultEmployeesPredicate = (employee: Employee) => userEmployee && employee.departmentId === userEmployee.departmentId;

        return ({
            employees: ownProps.employees,
            userEmployee,
            employeesPredicate: ownProps.customEmployeesPredicate ? ownProps.customEmployeesPredicate : defaultEmployeesPredicate
        });
    };

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee))
});

export class PeopleDepartmentImpl extends React.Component<NavigationScreenProps & PeopleDepartmentStateProps & EmployeesListDispatchProps & PeopleDepartmentPropsOwnProps> {
    
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
        const employees = this.props.employees.employeesById.filter(this.props.employeesPredicate).toArray();

        return <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked} navigation={this.props.navigation}/>;
    }

}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps, null, {withRef: true})(PeopleDepartmentImpl);
