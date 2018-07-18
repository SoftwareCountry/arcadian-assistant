import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap, EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction, CurrentDepartmentNavigationParams } from '../employee-details/employee-details-dispatcher';
import { NavigationRoute, NavigationScreenProp, NavigationLeafRoute } from 'react-navigation';

interface NavigationTyped<P> extends NavigationScreenProp<NavigationRoute> {
    getParam: <T extends keyof P>(param: T, fallback?: P[T]) => P[T];
}

interface NavigationProps {
    navigation: NavigationTyped<CurrentDepartmentNavigationParams>;
}

interface PeopleDepartmentProps {
    employees: EmployeesStore;
    userEmployee: Employee;
    employeesPredicate: (employee: Employee) => boolean;
}

const mapStateToProps = (state: AppState, ownProps: NavigationProps): PeopleDepartmentProps => {

    const userEmployee = state.organization.employees.employeesById.get(state.userInfo.employeeId);
    const departmentId = ownProps.navigation.getParam('departmentId', undefined);

    return {
        employees: state.organization.employees,
        userEmployee,
        employeesPredicate: (employee: Employee) => {

            return departmentId 
                ? employee.departmentId === departmentId 
                : userEmployee && employee.departmentId === userEmployee.departmentId;
        }
    };
};

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee))
});

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps & EmployeesListDispatchProps & NavigationProps> {
    public shouldComponentUpdate(nextProps: PeopleDepartmentProps & EmployeesListDispatchProps & NavigationProps) {
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
        return <EmployeesList employees={this.props.employees.employeesById.toArray().filter(this.props.employeesPredicate)} onItemClicked={this.props.onItemClicked} />;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);