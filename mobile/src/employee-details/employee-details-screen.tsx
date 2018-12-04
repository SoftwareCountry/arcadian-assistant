import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { SafeAreaView, ViewStyle } from 'react-native';
import { profileScreenStyles } from '../profile/styles';
import { EmployeeDetails } from './employee-details';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { NavigationScreenProps } from 'react-navigation';
import { LoadingView } from '../navigation/loading';

//============================================================================
interface EmployeeDetailsProps {
    departments: Department[];
}

//============================================================================
class EmployeeDetailsScreenImpl extends Component<EmployeeDetailsProps & NavigationScreenProps> {

    //----------------------------------------------------------------------------
    public render() {
        const employee = this.props.navigation.getParam('employee', undefined);

        const department = this.props.departments.find((d: Department) => d.departmentId === employee.departmentId);

        return employee && department ?
            <SafeAreaView style={profileScreenStyles.profileContainer}>
                <EmployeeDetails
                    department={department}
                    employee={employee}
                    layoutStylesChevronPlaceholder={layoutStylesForEmployeeDetailsScreen.chevronPlaceholder as ViewStyle}
                />
            </SafeAreaView> :
            <LoadingView/>;
    }
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): EmployeeDetailsProps => ({
    departments: state.organization ? state.organization.departments : Array(),
});

export const EmployeeDetailsScreen = connect(stateToProps)(EmployeeDetailsScreenImpl);
