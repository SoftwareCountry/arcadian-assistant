import { NavigationActions } from 'react-navigation';
import { Dispatch } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';

export const openEmployeeDetailsAction = (employee: Employee) => NavigationActions.navigate({
    routeName: 'CurrentProfile',
    params: {employee},
});