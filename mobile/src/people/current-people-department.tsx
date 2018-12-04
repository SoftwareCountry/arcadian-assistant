import React from 'react';
import { NavigationScreenConfig, NavigationScreenProps, NavigationStackScreenOptions } from 'react-navigation';
import { defaultState, EmployeesStore } from '../reducers/organization/employees.reducer';
import { PeopleDepartment } from './people-department';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';
import { SafeAreaView } from 'react-native';
import Style from '../layout/style';

//============================================================================
interface CurrentPeopleDepartmentProps {
    employees: EmployeesStore;
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): CurrentPeopleDepartmentProps => ({
    employees: state.organization ? state.organization.employees : defaultState,
});

//============================================================================
class CurrentPeopleDepartmentImpl extends React.Component<CurrentPeopleDepartmentProps & NavigationScreenProps> {
    public render() {
        const departmentId = this.props.navigation.getParam('departmentId', undefined);
        const customEmployeesPredicate = (employee: Employee) => employee.departmentId === departmentId;

        return <SafeAreaView style={Style.view.safeArea}>
            <PeopleDepartment
                customEmployeesPredicate={customEmployeesPredicate}
                employees={this.props.employees}/>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    public static navigationOptions: NavigationScreenConfig<NavigationStackScreenOptions> = (navigationOptionsContainer) => {
        const navigation = navigationOptionsContainer.navigation;
        const departmentAbbreviation = navigation.getParam('departmentAbbreviation', undefined);

        return {
            ...navigationOptionsContainer.navigationOptions,
            headerTitle: departmentAbbreviation ? `${departmentAbbreviation}` : '',
        };
    };
}

//----------------------------------------------------------------------------
export const CurrentPeopleDepartment = connect(stateToProps)(CurrentPeopleDepartmentImpl);
