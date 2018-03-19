import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { StyledText } from '../override/styled-text';
import { View, SafeAreaView } from 'react-native';
import { profileScreenStyles } from '../profile/styles';
import { EmployeeDetails } from './employee-details';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { NavigationRoute } from 'react-navigation';
import { Employee } from '../reducers/organization/employee.model';
 
interface EmployeeDetailsProps {
    departments: Department[]; 
}

interface NavigationProps {
    navigation: {
        state: NavigationRoute //TODO: this should be taken from redux state instead.
    };
}

const mapStateToProps = (state: AppState): EmployeeDetailsProps => ({
    departments: state.organization.departments,
});

export class EmployeeDetailsScreenImpl extends Component<EmployeeDetailsProps & NavigationProps> {
    public render() {
        const employee = this.props.navigation.state.params.employee as Employee;

        const department = this.props.departments.find((d: Department) => d.departmentId === employee.departmentId);

        return employee && department ?
            <SafeAreaView style={profileScreenStyles.profileContainer}>
                <EmployeeDetails department={department} employee={employee} layoutStylesChevronPlaceholder = {layoutStylesForEmployeeDetailsScreen.chevronPlaceholder}/>
            </SafeAreaView>
            : (
                <View style={profileScreenStyles.loadingContainer}>
                    <StyledText style={profileScreenStyles.loadingText}>Loading...</StyledText>
                </View>
            );
    }
}

export const EmployeeDetailsScreen = connect(mapStateToProps)(EmployeeDetailsScreenImpl);