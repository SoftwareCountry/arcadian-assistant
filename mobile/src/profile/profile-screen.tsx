import React, { Component } from 'react';

import { StackNavigator } from 'react-navigation';
import { Department } from '../reducers/organization/department.model';
import { UserInfoState } from '../reducers/user/user-info.reducer';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { StyledText } from '../override/styled-text';
import { View, SafeAreaView } from 'react-native';
import { Employee } from '../reducers/organization/employee.model';
import { chevronColor, profileScreenStyles, layoutStyles } from './styles';
import { EmployeeDetails } from '../employee-details/employee-details';
 
interface ProfileScreenProps {
    employee: Employee;
    departments: Department[];
}

const mapStateToProps = (state: AppState): ProfileScreenProps => ({
    employee: state.userInfo.employee,
    departments: state.organization.departments
});

class ProfileScreenImpl extends Component<ProfileScreenProps> {
    public render() {
        const employee = this.props.employee;
        const department = this.props.departments && employee ? this.props.departments.find((d) => d.departmentId === employee.departmentId) : null;

        return employee && department ?
            <SafeAreaView style={profileScreenStyles.profileContainer}>
               <EmployeeDetails department={department} employee={employee} layoutStylesChevronPlaceholder = {layoutStyles.chevronPlaceholder}/>
            </SafeAreaView>
            : (
                <View style={profileScreenStyles.loadingContainer}>
                    <StyledText style={profileScreenStyles.loadingText}>Loading...</StyledText>
                </View>
            );
    }
}

export const ProfileScreen = connect(mapStateToProps)(ProfileScreenImpl);