import React from 'react';
import { NavigationRoute, NavigationScreenProp, NavigationLeafRoute } from 'react-navigation';
import { CurrentDepartmentNavigationParams } from '../employee-details/employee-details-dispatcher';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { PeopleDepartment } from './people-department';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';
import { PeopleDepartmentFiltered } from './people-filtered';

interface ExtendedNavigationScreenProp<P> extends NavigationScreenProp<NavigationRoute> {
    getParam: <T extends keyof P>(param: T, fallback?: P[T]) => P[T];
}

interface NavigationProps {
    navigation: ExtendedNavigationScreenProp<CurrentDepartmentNavigationParams>;
}

interface CurrentPeopleDepartmentProps {
    employees: EmployeesStore;
}

const mapStateToProps = (state: AppState): CurrentPeopleDepartmentProps => ({
    employees: state.organization.employees
});

class CurrentPeopleDepartmentImpl extends React.Component<CurrentPeopleDepartmentProps & NavigationProps> {
    public render() {
        const departmentId = this.props.navigation.getParam('departmentId', undefined);
        const customEmployeesPredicate = (employee: Employee) => employee.departmentId === departmentId;

        return <PeopleDepartment customEmployeesPredicate={customEmployeesPredicate} employees={this.props.employees} navigation={this.props.navigation}/>;
    }
}

export const CurrentPeopleDepartment = connect(mapStateToProps)(CurrentPeopleDepartmentImpl);
