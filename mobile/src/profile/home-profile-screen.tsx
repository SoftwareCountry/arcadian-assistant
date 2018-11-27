import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { RefreshControl, SafeAreaView, ScrollView, StyleSheet, View } from 'react-native';
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

//============================================================================
interface ProfileScreenProps {
    employees: EmployeesStore;
    employee: Employee;
    departments: Department[];
    requests: Map<string, CalendarEvent[]>;
}

//============================================================================
interface AuthDispatchProps {
    refresh: () => void;
    loadPendingRequests: () => void;
}

//============================================================================
const getDepartment = (departments?: Department[], employee?: Employee): Department | null => {
    if (!departments || !employee) {
        return null;
    }
    return departments.find((d) => d.departmentId === employee.departmentId);
};

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
        const requests = this.props.requests;
        const nextRequests = nextProps.requests;
        if (!requests.equals(nextRequests)) {
            const calendarEvents = requests.get(this.props.employee.employeeId);
            const nextCalendarEvents = nextRequests.get(nextProps.employee.employeeId);

            if (calendarEvents !== nextCalendarEvents) {
                return true;
            }
        }

        const employees = this.props.employees.employeesById;
        const nextEmployees = nextProps.employees.employeesById;
        if (!employees.equals(nextEmployees)) {
            const newEmployeesSubset = nextEmployees.filter(element => {
                return !employees.has(element.employeeId);
            });

            if (requests.keySeq().some(key => newEmployeesSubset.has(key))) {
                return true;
            }
        }

        const employee = this.props.employee;
        const nextEmployee = nextProps.employee;
        if (nextEmployee && !nextEmployee.equals(employee)) {
            return true;
        }

        const department = getDepartment(this.props.departments, employee);
        const nextDepartment = getDepartment(nextProps.departments, employee);
        if (nextDepartment && !nextDepartment.equals(department)) {
            return true;
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
        const department = getDepartment(this.props.departments, employee);
        const employeesToRequests = this.props.requests.mapKeys(employeeId => this.props.employees.employeesById.get(employeeId)).toMap();

        return employee && department ?
            <ScrollView refreshControl={<RefreshControl refreshing={false} onRefresh={this.onRefresh}/>}>
                <EmployeeDetails
                    department={department}
                    employee={employee}
                    layoutStylesChevronPlaceholder={layoutStyles.chevronPlaceholder}
                    requests={employeesToRequests}
                />
            </ScrollView>
            : <LoadingView/>;
    }

    //----------------------------------------------------------------------------
    private onRefresh = () => {
        this.props.refresh();
    };
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): ProfileScreenProps => ({
    employees: state.organization.employees,
    employee: state.organization.employees.employeesById.get(state.userInfo.employeeId),
    departments: state.organization.departments,
    requests: state.calendar.pendingRequests.requests
});

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<AuthActions>): AuthDispatchProps => ({
    refresh: () => dispatch(refresh()),
    loadPendingRequests: () => dispatch(loadPendingRequests()),
});

//----------------------------------------------------------------------------
export const HomeProfileScreen = connect(stateToProps, dispatchToProps)(ProfileScreenImpl);
