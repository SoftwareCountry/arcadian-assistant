import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { StyledText } from '../override/styled-text';
import { View, SafeAreaView, ScrollView, RefreshControl } from 'react-native';
import { Employee } from '../reducers/organization/employee.model';
import { profileScreenStyles, layoutStyles } from './styles';
import { EmployeeDetails } from '../employee-details/employee-details';
import { AuthActions } from '../reducers/auth/auth.action';
import { refresh } from '../reducers/refresh/refresh.action';
import { loadPendingRequests } from '../reducers/calendar/pending-requests/pending-requests.action';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Map } from 'immutable';
import { CalendarEvent } from '../reducers/calendar/calendar-event.model';
import { LogoutView } from '../navigation/logout-view';

interface ProfileScreenProps {
    employees: EmployeesStore;
    employee: Employee;
    departments: Department[];
    requests: Map<string, CalendarEvent[]>;
}

const mapStateToProps = (state: AppState): ProfileScreenProps => ({
    employees: state.organization.employees,
    employee: state.organization.employees.employeesById.get(state.userInfo.employeeId),
    departments: state.organization.departments,
    requests: state.calendar.pendingRequests.requests
});

interface AuthDispatchProps {
    refresh: () => void;
    loadPendingRequests: () => void;
}
const mapDispatchToProps = (dispatch: Dispatch<AuthActions>): AuthDispatchProps => ({
    refresh: () => dispatch(refresh()),
    loadPendingRequests: () => dispatch(loadPendingRequests()),
});

class ProfileScreenImpl extends Component<ProfileScreenProps & AuthDispatchProps> {
    public componentDidMount() {
        this.props.loadPendingRequests();
    }

    public shouldComponentUpdate(nextProps: ProfileScreenProps & AuthDispatchProps) {
        const employees = this.props.employees.employeesById;
        const nextEmployees = nextProps.employees.employeesById;
        const requests = this.props.requests;
        const nextRequests = nextProps.requests;

        if (!requests.equals(nextRequests)) {
            const calendarEvents = requests.get(this.props.employee.employeeId);
            const nextCalendarEvents = nextRequests.get(nextProps.employee.employeeId);

            return calendarEvents !== nextCalendarEvents;
        }

        if (!employees.equals(nextEmployees)) {
            const newEmployeesSubset = nextEmployees.filter(employee => {
                return !employees.has(employee.employeeId);
            });

            return requests.keySeq().some(key => newEmployeesSubset.has(key));
        }

        return false;
    }

    public render() {
        const employee = this.props.employee;
        const department = this.props.departments && employee ? this.props.departments.find((d) => d.departmentId === employee.departmentId) : null;
        const employeesToRequests = this.props.requests.mapKeys(employeeId => this.props.employees.employeesById.get(employeeId)).toMap();        

        return employee && department ?
            <ScrollView refreshControl= { <RefreshControl refreshing={false} onRefresh= {this.onRefresh} />} >
                <SafeAreaView style={profileScreenStyles.profileContainer}>
                    <LogoutView/>
                    <EmployeeDetails 
                        department={department} 
                        employee={employee} 
                        layoutStylesChevronPlaceholder={layoutStyles.chevronPlaceholder} 
                        requests={employeesToRequests}
                    />
                </SafeAreaView>
            </ScrollView>
            : (
                <View style={profileScreenStyles.loadingContainer}>
                    <StyledText style={profileScreenStyles.loadingText}>Loading...</StyledText>
                </View>
            );
    }

    private onRefresh = () => {    
        this.props.refresh();
    }
}

export const ProfileScreen = connect(mapStateToProps, mapDispatchToProps)(ProfileScreenImpl);