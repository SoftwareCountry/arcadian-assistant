import React from 'react';
import {
    NavigationRoute,
    NavigationScreenConfig,
    NavigationScreenProp,
    NavigationStackScreenOptions
} from 'react-navigation';
import {CurrentRoomNavigationParams} from '../employee-details/employee-details-dispatcher';
import {EmployeesStore} from '../reducers/organization/employees.reducer';
import {AppState} from '../reducers/app.reducer';
import {connect} from 'react-redux';
import {Employee} from '../reducers/organization/employee.model';
import {PeopleRoom} from './people-room';
import {Action, Dispatch} from 'redux';
import {loadEmployeesForRoom} from '../reducers/organization/organization.action';
import topNavBarStyles from '../navigation/top-nav-bar-styles';
import { Platform, StatusBar, View } from 'react-native';
import { StyledText } from '../override/styled-text';

interface ExtendedNavigationScreenProp<P> extends NavigationScreenProp<NavigationRoute> {
    getParam: <T extends keyof P>(param: T, fallback?: P[T]) => P[T];
}

interface NavigationProps {
    navigation: ExtendedNavigationScreenProp<CurrentRoomNavigationParams>;
}

interface CurrentPeopleRoomProps {
    employees: EmployeesStore;
}

interface CurrentPeopleDispatchProps {
    loadEmployeesForRoom: (roomNumber: string) => void;
}

const mapStateToProps = (state: AppState): CurrentPeopleRoomProps => ({
    employees: state.organization.employees
});

const dispatchToProps = (dispatch: Dispatch<Action>) => {
    return {
        loadEmployeesForRoom: (roomNumber: string) => {
            dispatch(loadEmployeesForRoom(roomNumber));
        },
    };
};

class CurrentPeopleRoomImpl extends React.Component<CurrentPeopleRoomProps & CurrentPeopleDispatchProps & NavigationProps> {
    public static navigationOptions: NavigationScreenConfig<NavigationStackScreenOptions> = {
        headerTitle: 'Header Title',
        headerTitleStyle: {
            fontFamily: 'CenturyGothic',
            fontSize: 14,
            color: 'white',
            textAlign: 'center'
        },
        headerStyle: {
            height: Platform.OS === 'ios' ? 50 : 30,
            marginTop: 0,
            backgroundColor: '#2FAFCC'
        }
    };

    public componentDidMount() {
        const roomNumber = this.props.navigation.getParam('roomNumber', undefined);
        this.props.loadEmployeesForRoom(roomNumber);
    }

    public render() {
        const roomNumber = this.props.navigation.getParam('roomNumber', undefined);
        const customEmployeesPredicate = (employee: Employee) => employee.roomNumber === roomNumber;

        return <PeopleRoom customEmployeesPredicate={customEmployeesPredicate} employees={this.props.employees}/>;
    }
}

export const CurrentPeopleRoom = connect(mapStateToProps, dispatchToProps)(CurrentPeopleRoomImpl);
