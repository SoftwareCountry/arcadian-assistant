import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { StyledText } from '../override/styled-text';
import { View, SafeAreaView } from 'react-native';
import { profileScreenStyles } from '../profile/styles';
import { EmployeeDetails } from './employee-details';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
 
interface EmployeeDetailsProps {
    departments: Department[];
}

const mapStateToProps = (state: AppState): EmployeeDetailsProps => ({
    departments: state.organization.departments
});

export class EmployeeDetailsScreenImpl extends Component<EmployeeDetailsProps &  any > {
    public render() {
      
        const employee = this.props.navigation.state.params.employee;

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