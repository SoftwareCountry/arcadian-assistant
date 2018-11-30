import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { SafeAreaView, StyleSheet, ViewStyle } from 'react-native';
import { profileScreenStyles } from '../profile/styles';
import { EmployeeDetails } from './employee-details';
import { layoutStylesForEmployeeDetailsScreen } from './styles';
import { NavigationScreenConfig, NavigationScreenProps, NavigationStackScreenOptions } from 'react-navigation';
import { LoadingView } from '../navigation/loading';
import Style from '../layout/style';

interface EmployeeDetailsProps {
    departments: Department[];
}

const mapStateToProps = (state: AppState): EmployeeDetailsProps => ({
    departments: state.organization ? state.organization.departments : Array(),
});

export class EmployeeDetailsScreenImpl extends Component<EmployeeDetailsProps & NavigationScreenProps> {

    //----------------------------------------------------------------------------
    public static navigationOptions: NavigationScreenConfig<NavigationStackScreenOptions> = {
        headerStyle: {
            ...StyleSheet.flatten(Style.navigation.header),
            borderBottomColor: 'transparent',
        },
    };

    public render() {
        const employee = this.props.navigation.getParam('employee', undefined);

        const department = this.props.departments.find((d: Department) => d.departmentId === employee.departmentId);

        return employee && department ?
            <SafeAreaView style={profileScreenStyles.profileContainer}>
                <EmployeeDetails
                    department={department}
                    employee={employee}
                    layoutStylesChevronPlaceholder = {layoutStylesForEmployeeDetailsScreen.chevronPlaceholder as ViewStyle}
                />
            </SafeAreaView>
            : <LoadingView></LoadingView>;
    }
}

export const EmployeeDetailsScreen = connect(mapStateToProps)(EmployeeDetailsScreenImpl);
