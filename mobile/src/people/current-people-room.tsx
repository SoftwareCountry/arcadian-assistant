import React from 'react';
import {
    Header, HeaderProps, NavigationParams,
    NavigationRoute,
    NavigationScreenConfig,
    NavigationScreenProp,
    NavigationStackScreenOptions
} from 'react-navigation';
import { CurrentRoomNavigationParams } from '../employee-details/employee-details-dispatcher';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';
import { PeopleRoom } from './people-room';
import { Action, Dispatch } from 'redux';
import { loadEmployeesForRoom } from '../reducers/organization/organization.action';
import { SafeAreaView, View } from 'react-native';
import { StyledText } from '../override/styled-text';

//============================================================================
interface ExtendedNavigationScreenProp<P> extends NavigationScreenProp<NavigationRoute> {
    getParam: <T extends keyof P>(param: T, fallback?: P[T]) => P[T];
}

//============================================================================
interface NavigationProps {
    navigation: ExtendedNavigationScreenProp<CurrentRoomNavigationParams>;
}

//============================================================================
interface CurrentPeopleRoomProps {
    employees: EmployeesStore;
}

//============================================================================
interface CurrentPeopleDispatchProps {
    loadEmployeesForRoom: (roomNumber: string) => void;
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): CurrentPeopleRoomProps => ({
    employees: state.organization.employees
});

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>) => {
    return {
        loadEmployeesForRoom: (roomNumber: string) => {
            dispatch(loadEmployeesForRoom(roomNumber));
        },
    };
};

//============================================================================
class CurrentPeopleRoomImpl extends React.Component<CurrentPeopleRoomProps & CurrentPeopleDispatchProps & NavigationProps> {
    //----------------------------------------------------------------------------
    public componentDidMount() {
        const roomNumber = this.props.navigation.getParam('roomNumber', undefined);
        this.props.loadEmployeesForRoom(roomNumber);
    }

    //----------------------------------------------------------------------------
    public render() {
        const roomNumber = this.props.navigation.getParam('roomNumber', undefined);
        const customEmployeesPredicate = (employee: Employee) => employee.roomNumber === roomNumber;

        return <SafeAreaView>
            <PeopleRoom
                customEmployeesPredicate={customEmployeesPredicate}
                employees={this.props.employees}/>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    public static navigationOptions: NavigationScreenConfig<NavigationStackScreenOptions> = (navigationOptionsContainer) => {
        const navigation = navigationOptionsContainer.navigation as ExtendedNavigationScreenProp<NavigationParams>;
        const roomNumber = navigation.getParam('roomNumber', undefined);

        return {
            headerTitle: roomNumber ? `Room ${roomNumber}` : '',
            headerTitleStyle: {
                fontFamily: 'CenturyGothic',
                fontSize: 14,
                color: 'white',
                textAlign: 'center'
            },
            headerStyle: {
                backgroundColor: '#2FAFCC'
            }
        };
    };
}

//----------------------------------------------------------------------------
export const CurrentPeopleRoom = connect(stateToProps, dispatchToProps)(CurrentPeopleRoomImpl);
