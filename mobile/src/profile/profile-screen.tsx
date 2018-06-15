import React, { Component } from 'react';

import { StackNavigator } from 'react-navigation';
import { Department } from '../reducers/organization/department.model';
import { UserInfoState } from '../reducers/user/user-info.reducer';
import { AppState } from '../reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { StyledText } from '../override/styled-text';
import { View, SafeAreaView, TouchableOpacity, Image } from 'react-native';
import { Employee } from '../reducers/organization/employee.model';
import { chevronColor, profileScreenStyles, layoutStyles } from './styles';
import { EmployeeDetails } from '../employee-details/employee-details';
import { AuthActions, startLogoutProcess } from '../reducers/auth/auth.action';
 
interface ProfileScreenProps {
    employee: Employee;
    departments: Department[];
}

const mapStateToProps = (state: AppState): ProfileScreenProps => ({
    employee: state.organization.employees.employeesById.get(state.userInfo.employeeId),
    departments: state.organization.departments
});

interface AuthDispatchProps {
    onlogoutClicked: () => void;
}
const mapDispatchToProps = (dispatch: Dispatch<AuthActions>): AuthDispatchProps => ({
    onlogoutClicked: () => { dispatch(startLogoutProcess()); }
});

class ProfileScreenImpl extends Component<ProfileScreenProps & AuthDispatchProps> {
    public render() {
        const employee = this.props.employee;
        const department = this.props.departments && employee ? this.props.departments.find((d) => d.departmentId === employee.departmentId) : null;

        return employee && department ?
            <SafeAreaView style={profileScreenStyles.profileContainer}>
                <TouchableOpacity onPress={this.props.onlogoutClicked}>
                    <View style={layoutStyles.logoutContainer}>
                        <Image style={profileScreenStyles.imageLogout} source={require('./logout-image.png')} />
                    </View>
                </TouchableOpacity>
                <EmployeeDetails department={department} employee={employee} layoutStylesChevronPlaceholder={layoutStyles.chevronPlaceholder} showPendingRequests />
            </SafeAreaView>
            : (
                <View style={profileScreenStyles.loadingContainer}>
                    <StyledText style={profileScreenStyles.loadingText}>Loading...</StyledText>
                </View>
            );
    }
}

export const ProfileScreen = connect(mapStateToProps, mapDispatchToProps)(ProfileScreenImpl);