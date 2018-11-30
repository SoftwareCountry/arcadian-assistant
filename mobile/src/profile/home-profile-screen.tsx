import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import {RefreshControl, SafeAreaView, ScrollView, StyleSheet, View, ViewStyle} from 'react-native';
import { Employee } from '../reducers/organization/employee.model';
import { layoutStyles, profileScreenStyles } from './styles';
import { EmployeeDetails } from '../employee-details/employee-details';
import { AuthActions } from '../reducers/auth/auth.action';
import { refresh } from '../reducers/refresh/refresh.action';
import { loadPendingRequests } from '../reducers/calendar/pending-requests/pending-requests.action';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Map } from 'immutable';
import { CalendarEvent } from '../reducers/calendar/calendar-event.model';
import { LogoutView } from '../navigation/logout-view';
import { LoadingView } from '../navigation/loading';
import Style from '../layout/style';
import { NavigationScreenConfig, NavigationStackScreenOptions } from 'react-navigation';
import { Action, Dispatch } from 'redux';
import { Optional } from 'types';

//============================================================================
interface ProfileScreenProps {
    employees: Optional<EmployeesStore>;
    employee: Optional<Employee>;
    department: Optional<Department>;
    requests: Optional<Map<string, CalendarEvent[]>>;
}

//============================================================================
interface AuthDispatchProps {
    refresh: () => void;
    loadPendingRequests: () => void;
}

//============================================================================
class ProfileScreenImpl extends Component<ProfileScreenProps & AuthDispatchProps> {

    //----------------------------------------------------------------------------
    public static navigationOptions: NavigationScreenConfig<NavigationStackScreenOptions> = {
        headerStyle: {
            ...StyleSheet.flatten(Style.navigation.header),
            borderBottomColor: 'transparent',
        },
        headerRight: <LogoutView/>,
    };

    //----------------------------------------------------------------------------
    public componentDidMount() {
        this.props.loadPendingRequests();
    }

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: ProfileScreenProps & AuthDispatchProps) {
        if (!this.props.requests && !nextProps.requests) {
            return false;
        }

        if (!this.props.requests || !nextProps.requests) {
            return true;
        }

        const requests = this.props.requests;
        const nextRequests = nextProps.requests;
        if (!requests.equals(nextRequests)) {
            const employee = this.props.employee;
            const nextEmployee = nextProps.employee;
            if (employee && nextEmployee) {
                const calendarEvents = requests.get(employee.employeeId);
                const nextCalendarEvents = nextRequests.get(nextEmployee.employeeId);

                if (calendarEvents !== nextCalendarEvents) {
                    return true;
                }
            }
        }

        if (!this.props.employees && !nextProps.employees) {
            return false;
        }

        if (!this.props.employees || !nextProps.employees) {
            return true;
        }

        const employees = this.props.employees.employeesById;
        const nextEmployees = nextProps.employees.employeesById;
        if (!employees.equals(nextEmployees)) {
            const newEmployeesSubset = nextEmployees.filter(employee => {
                return !employees.has(employee.employeeId);
            });

            return requests.keySeq().some(key => newEmployeesSubset.has(key));
        }

        return false;
    }

    //----------------------------------------------------------------------------
    public render() {
        return <SafeAreaView style={Style.view.safeArea}>
            <View style={profileScreenStyles.employeeDetailsContainer}>
                {this.renderEmployeeDetails()}
            </View>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    private renderEmployeeDetails() {
        const employee = this.props.employee;
        const employees = this.props.employees;
        const department = this.props.department;
        if (!employee || !employees || !department) {
            return <LoadingView/>;
        }

        const requests = this.props.requests;
        const employeesToRequests = requests ? requests
            .filter((event, employeeId) => { return employees.employeesById.get(employeeId) !== undefined })
            .mapKeys(employeeId => employees.employeesById.get(employeeId)!) : undefined;

        return (
            <ScrollView refreshControl={<RefreshControl refreshing={false} onRefresh={this.onRefresh}/>}>
                <EmployeeDetails
                    department={department}
                    employee={employee}
                    layoutStylesChevronPlaceholder={layoutStyles.chevronPlaceholder as ViewStyle}
                    requests={employeesToRequests}
                />
            </ScrollView>
        );
    }

    //----------------------------------------------------------------------------
    private onRefresh = () => {
        this.props.refresh();
    };
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): ProfileScreenProps => {
    function getEmployeesStore(state: AppState): Optional<EmployeesStore> {
        return state.organization ? state.organization.employees : undefined;
    }

    function getEmployee(state: AppState): Optional<Employee> {
        return (state.organization && state.userInfo && state.userInfo.employeeId) ?
            state.organization.employees.employeesById.get(state.userInfo.employeeId) :
            undefined;
    }

    function getDepartment(state: AppState, employee: Optional<Employee>): Optional<Department> {
        if (!state.organization || !employee) {
            return undefined;
        }

        const departments = state.organization.departments;
        return departments && employee ?
            departments.find((d) => d.departmentId === employee.departmentId) :
            undefined;
    }

    const employee = getEmployee(state);

    return {
        employees: getEmployeesStore(state),
        employee: employee,
        department: getDepartment(state, employee),
        requests: state.calendar ? state.calendar.pendingRequests.requests : undefined,
    };
};

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>): AuthDispatchProps => ({
    refresh: () => dispatch(refresh()),
    loadPendingRequests: () => dispatch(loadPendingRequests()),
});

//----------------------------------------------------------------------------
export const HomeProfileScreen = connect(stateToProps, dispatchToProps)(ProfileScreenImpl);
