import React from 'react';
import {
    Header, HeaderProps, NavigationParams,
    NavigationRoute,
    NavigationScreenConfig,
    NavigationScreenProp, NavigationScreenProps,
    NavigationStackScreenOptions
} from 'react-navigation';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';
import { PeopleRoom } from './people-room';
import { Action, Dispatch } from 'redux';
import { loadEmployeesForRoom } from '../reducers/organization/organization.action';
import { SafeAreaView, View } from 'react-native';
import { StyledText } from '../override/styled-text';
import Style from '../layout/style';
import { Map } from 'immutable';

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
    employees: state.organization ? state.organization.employees : {
        employeesById: Map(),
        employeeIdsByDepartment: Map()
    }
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
class CurrentPeopleRoomImpl extends React.Component<CurrentPeopleRoomProps & CurrentPeopleDispatchProps & NavigationScreenProps> {
    //----------------------------------------------------------------------------
    public componentDidMount() {
        const roomNumber = this.props.navigation.getParam('roomNumber', undefined);
        this.props.loadEmployeesForRoom(roomNumber);
    }

    //----------------------------------------------------------------------------
    public render() {
        const roomNumber = this.props.navigation.getParam('roomNumber', undefined);
        const customEmployeesPredicate = (employee: Employee) => employee.roomNumber === roomNumber;

        return <SafeAreaView style={Style.view.safeArea}>
            <PeopleRoom
                customEmployeesPredicate={customEmployeesPredicate}
                employees={this.props.employees}/>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    public static navigationOptions: NavigationScreenConfig<NavigationStackScreenOptions> = (navigationOptionsContainer) => {
        const navigation = navigationOptionsContainer.navigation;
        const roomNumber = navigation.getParam('roomNumber', undefined);

        return {
            headerTitle: roomNumber ? `Room ${roomNumber}` : '',
            headerTitleStyle: Style.navigation.title,
            headerStyle: Style.navigation.header,
        };
    };
}

//----------------------------------------------------------------------------
export const CurrentPeopleRoom = connect(stateToProps, dispatchToProps)(CurrentPeopleRoomImpl);
