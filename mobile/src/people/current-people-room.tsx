import React from 'react';
import { NavigationScreenConfig, NavigationScreenProps, NavigationStackScreenOptions } from 'react-navigation';
import { defaultState, EmployeesStore } from '../reducers/organization/employees.reducer';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';
import { PeopleRoom } from './people-room';
import { Action, Dispatch } from 'redux';
import { loadEmployeesForRoom } from '../reducers/organization/organization.action';
import { SafeAreaView } from 'react-native';
import Style from '../layout/style';
import {
    NavigationOptionsContainer,
    navigationOptionsWithTitle
} from '../navigation/navigation-header-with-dynamic-title';

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
    employees: state.organization ? state.organization.employees : defaultState,
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
        return navigationOptionsWithTitle(navigationOptionsContainer, CurrentPeopleRoomImpl.getTitle(navigationOptionsContainer));
    };

    //----------------------------------------------------------------------------
    private static getTitle(navigationOptionsContainer: NavigationOptionsContainer): string {
        const navigation = navigationOptionsContainer.navigation;
        const roomNumber = navigation.getParam('roomNumber', undefined);
        return roomNumber ? `Room ${roomNumber}` : '';
    }
}

//----------------------------------------------------------------------------
export const CurrentPeopleRoom = connect(stateToProps, dispatchToProps)(CurrentPeopleRoomImpl);
