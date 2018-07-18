import React from 'react';
import { connect, Dispatch, MapStateToProps } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction, CurrentDepartmentNavigationParams } from '../employee-details/employee-details-dispatcher';
import { NavigationRoute, NavigationScreenProp, NavigationLeafRoute } from 'react-navigation';

interface ExtendedNavigationScreenProp<P> extends NavigationScreenProp<NavigationRoute> {
    getParam: <T extends keyof P>(param: T, fallback?: P[T]) => P[T];
}

interface NavigationProps {
    navigation: ExtendedNavigationScreenProp<CurrentDepartmentNavigationParams>;
}

interface PeopleDepartmentPropsOwnProps {
    employees: EmployeesStore;
}

interface PeopleDepartmentStateProps {
    employees: EmployeesStore;
    userEmployee: Employee;
    employeesPredicate: (employee: Employee) => boolean;
}

type PeopleDepartmentProps = PeopleDepartmentStateProps & PeopleDepartmentPropsOwnProps;

const mapStateToProps: MapStateToProps<PeopleDepartmentProps, PeopleDepartmentPropsOwnProps, AppState> = (state: AppState, ownProps: PeopleDepartmentPropsOwnProps & NavigationProps): PeopleDepartmentStateProps => {

    const userEmployee = state.organization.employees.employeesById.get(state.userInfo.employeeId);
    const departmentId = ownProps.navigation.getParam('departmentId', undefined);

    return ({
        employees: ownProps.employees,
        userEmployee,
        employeesPredicate: (employee: Employee) => {

            return departmentId 
                ? employee.departmentId === departmentId 
                : userEmployee && employee.departmentId === userEmployee.departmentId;
        }
    });
};

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee))
});

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentStateProps & EmployeesListDispatchProps & PeopleDepartmentPropsOwnProps> {
    public shouldComponentUpdate(nextProps: PeopleDepartmentStateProps & EmployeesListDispatchProps & PeopleDepartmentPropsOwnProps) {
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
        const employees = this.props.employees.employeesById.filter(this.props.employeesPredicate).toArray();

        return <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked} isLoading={this.props.employees.employeesById.size > 0}/>;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);