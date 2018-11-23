import React from 'react';
import {
    NavigationParams,
    NavigationRoute,
    NavigationScreenConfig,
    NavigationScreenProp,
    NavigationStackScreenOptions
} from 'react-navigation';
import { CurrentDepartmentNavigationParams } from '../employee-details/employee-details-dispatcher';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { PeopleDepartment } from './people-department';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';
import { SafeAreaView } from 'react-native';

//============================================================================
interface ExtendedNavigationScreenProp<P> extends NavigationScreenProp<NavigationRoute> {
    getParam: <T extends keyof P>(param: T, fallback?: P[T]) => P[T];
}

//============================================================================
interface NavigationProps {
    navigation: ExtendedNavigationScreenProp<CurrentDepartmentNavigationParams>;
}

//============================================================================
interface CurrentPeopleDepartmentProps {
    employees: EmployeesStore;
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): CurrentPeopleDepartmentProps => ({
    employees: state.organization.employees
});

//============================================================================
class CurrentPeopleDepartmentImpl extends React.Component<CurrentPeopleDepartmentProps & NavigationProps> {
    public render() {
        const departmentId = this.props.navigation.getParam('departmentId', undefined);
        const customEmployeesPredicate = (employee: Employee) => employee.departmentId === departmentId;

        return <SafeAreaView>
            <PeopleDepartment
                customEmployeesPredicate={customEmployeesPredicate}
                employees={this.props.employees}/>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    public static navigationOptions: NavigationScreenConfig<NavigationStackScreenOptions> = (navigationOptionsContainer) => {
        const navigation = navigationOptionsContainer.navigation as ExtendedNavigationScreenProp<NavigationParams>;
        const departmentAbbreviation = navigation.getParam('departmentAbbreviation', undefined);

        return {
            headerTitle: departmentAbbreviation ? `${departmentAbbreviation}` : '',
            headerTitleStyle: {
                fontFamily: 'CenturyGothic',
                fontSize: 14,
                color: 'white',
                textAlign: 'center'
            },
            headerStyle: {
                backgroundColor: '#2FAFCC'
            }
        };
    };
}

//----------------------------------------------------------------------------
export const CurrentPeopleDepartment = connect(stateToProps)(CurrentPeopleDepartmentImpl);
