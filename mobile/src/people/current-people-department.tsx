import React from 'react';
import { NavigationScreenConfig, NavigationScreenProps, NavigationStackScreenOptions } from 'react-navigation';
import { defaultState, EmployeesStore } from '../reducers/organization/employees.reducer';
import { PeopleDepartment } from './people-department';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';
import { SafeAreaView } from 'react-native';
import Style from '../layout/style';
import {
    NavigationOptionsContainer,
    navigationOptionsWithTitle
} from '../navigation/navigation-header-with-dynamic-title';

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
        return navigationOptionsWithTitle(navigationOptionsContainer, CurrentPeopleDepartmentImpl.getTitle(navigationOptionsContainer));
    };

    //----------------------------------------------------------------------------
    private static getTitle(navigationOptionsContainer: NavigationOptionsContainer): string {
        const navigation = navigationOptionsContainer.navigation;
        return navigation.getParam('departmentAbbreviation', '');
    }
}

//----------------------------------------------------------------------------
export const CurrentPeopleDepartment = connect(stateToProps)(CurrentPeopleDepartmentImpl);
