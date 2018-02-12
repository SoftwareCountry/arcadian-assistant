import React, { Component } from 'react';

import { StackNavigator } from 'react-navigation';
import { Profile } from './profile';
import { Department } from '../reducers/organization/department.model';
import { UserInfoState } from '../reducers/user/user-info.reducer';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { StyledText } from '../override/styled-text';
import { View, StyleSheet } from 'react-native';
import { Employee } from '../reducers/organization/employee.model';

const styles = StyleSheet.create({
    loadingContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    loadingText: {
        fontSize: 20
    }
});

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
            <Profile department={department} employee={employee} />
            : (
                <View style={styles.loadingContainer}>
                    <StyledText style={styles.loadingText}>Loading...</StyledText>
                </View>
            );
    }
}

export const ProfileScreen = connect(mapStateToProps)(ProfileScreenImpl);