import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState, getDepartment, getEmployee, getEmployees } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { LayoutChangeEvent, RefreshControl, SafeAreaView, StyleSheet, View, ViewStyle } from 'react-native';
import { Employee, EmployeeId } from '../reducers/organization/employee.model';
import { layoutStyles } from './styles';
import { EmployeeDetails } from '../employee-details/employee-details';
import { refresh } from '../reducers/refresh/refresh.action';
import { loadPendingRequests } from '../reducers/calendar/pending-requests/pending-requests.action';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LogoutView } from '../navigation/logout-view';
import { LoadingView } from '../navigation/loading';
import Style from '../layout/style';
import { NavigationEvents, NavigationScreenConfig, NavigationStackScreenOptions, ScrollView } from 'react-navigation';
import { Action, Dispatch } from 'redux';
import { Optional } from 'types';
import { SettingsView } from '../user-preferences-screen/settings-view';
import { employeeDetailsStyles } from '../employee-details/styles';
import { equals } from '../utils/equitable';
import { loadCalendarEvents } from '../reducers/calendar/calendar.action';

//============================================================================
interface ProfileScreenProps {
    employees: Optional<EmployeesStore>;
    employee: Optional<Employee>;
    department: Optional<Department>;
}

//============================================================================
interface AuthDispatchProps {
    refresh: () => void;
    loadPendingRequests: () => void;
    loadCalendarEvents: (employeeId: EmployeeId) => void;
}

//============================================================================
interface ProfileScreenState {
    width: number;
    height: number;
}

//============================================================================
class ProfileScreenImpl extends Component<ProfileScreenProps & AuthDispatchProps, ProfileScreenState> {

    //----------------------------------------------------------------------------
    public static navigationOptions: NavigationScreenConfig<NavigationStackScreenOptions> = {
        headerStyle: {
            ...StyleSheet.flatten(Style.navigation.header),
            borderBottomColor: 'transparent',
            elevation: 0,
        },
        headerLeft: <SettingsView/>,
        headerRight: <LogoutView/>,
    };

    //----------------------------------------------------------------------------
    public constructor(props: Readonly<ProfileScreenProps & AuthDispatchProps>) {
        super(props);

        this.state = {
            width: 0,
            height: 0,
        };
    }

    //----------------------------------------------------------------------------
    public componentDidMount() {
        this.props.loadPendingRequests();
    }

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: ProfileScreenProps & AuthDispatchProps, nextState: ProfileScreenState) {
        if (this.state.width !== nextState.width || this.state.height !== nextState.height) {
            return true;
        }

        const employee = this.props.employee;
        const nextEmployee = nextProps.employee;
        if (!equals(employee, nextEmployee)) {
            return true;
        }

        const department = this.props.department;
        const nextDepartment = nextProps.department;
        // noinspection RedundantIfStatementJS
        if (!equals(department, nextDepartment)) {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    public render() {
        return <SafeAreaView style={Style.view.safeArea} onLayout={this.onLayout}>
            <NavigationEvents
                onWillFocus={(_) => {
                    this.props.loadPendingRequests();

                    const { employee } = this.props;
                    if (employee) {
                        this.props.loadCalendarEvents(employee.employeeId);
                    }
                }}
            />

            <View style={employeeDetailsStyles.container}>

                <View style={[
                    employeeDetailsStyles.topHalfView,
                    {
                        width: this.state.width,
                        height: this.state.height / 2,
                    }]}/>
                <View style={[
                    employeeDetailsStyles.bottomHalfView,
                    {
                        top: this.state.height / 2,
                        width: this.state.width,
                        height: this.state.height / 2
                    }]}/>

                {this.renderEmployeeDetails()}

            </View>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    private renderEmployeeDetails(): JSX.Element {
        const employee = this.props.employee;
        const employees = this.props.employees;
        const department = this.props.department;

        if (!employee || !employees || !department) {
            return <LoadingView/>;
        }

        return (
            <ScrollView refreshControl={<RefreshControl tintColor={Style.color.white}
                                                        refreshing={false}
                                                        onRefresh={this.onRefresh}/>}
                        shouldCancelWhenOutside={false}>

                <EmployeeDetails
                    department={department}
                    employee={employee}
                    layoutStylesChevronPlaceholder={layoutStyles.chevronPlaceholder as ViewStyle}
                    showRequests={true}
                />

            </ScrollView>
        );
    }

    //----------------------------------------------------------------------------
    private onLayout = (event: LayoutChangeEvent) => {
        this.setState({
            width: event.nativeEvent.layout.width,
            height: event.nativeEvent.layout.height,
        });
    };

    //----------------------------------------------------------------------------
    private onRefresh = () => {
        this.props.refresh();
    };
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): ProfileScreenProps => {
    const employee = getEmployee(state);
    return {
        employees: getEmployees(state),
        employee: employee,
        department: getDepartment(state, employee),
    };
};

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>): AuthDispatchProps => ({
    refresh: () => dispatch(refresh()),
    loadPendingRequests: () => dispatch(loadPendingRequests()),
    loadCalendarEvents: employeeId => dispatch(loadCalendarEvents(employeeId)),
});

//----------------------------------------------------------------------------
export const HomeProfileScreen = connect(stateToProps, dispatchToProps)(ProfileScreenImpl);
