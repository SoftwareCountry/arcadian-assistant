import React from 'react';
import { NavigationRoute, NavigationScreenProp } from 'react-navigation';
import { CurrentRoomNavigationParams } from '../employee-details/employee-details-dispatcher';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';
import { PeopleRoom } from './people-room';

interface ExtendedNavigationScreenProp<P> extends NavigationScreenProp<NavigationRoute> {
    getParam: <T extends keyof P>(param: T, fallback?: P[T]) => P[T];
}

interface NavigationProps {
    navigation: ExtendedNavigationScreenProp<CurrentRoomNavigationParams>;
}

interface CurrentPeopleRoomProps {
    employees: EmployeesStore;
}

const mapStateToProps = (state: AppState): CurrentPeopleRoomProps => ({
    employees: state.organization.employees
});

class CurrentPeopleRoomImpl extends React.Component<CurrentPeopleRoomProps & NavigationProps> {
    public render() {
        const roomNumber = this.props.navigation.getParam('roomNumber', undefined);
        const customEmployeesPredicate = (employee: Employee) => employee.roomNumber === roomNumber;

        return <PeopleRoom customEmployeesPredicate={customEmployeesPredicate} employees={this.props.employees}/>;
    }
}

export const CurrentPeopleRoom = connect(mapStateToProps)(CurrentPeopleRoomImpl);