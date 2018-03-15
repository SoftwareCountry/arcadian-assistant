import { NavigationActions } from 'react-navigation';
import { Dispatch } from 'react-redux';
import { Employee } from '../reducers/organization/employee.model';

export interface EmployeeDetailsProps {
    onAvatarClicked: (employee: Employee) => void;
}

export const mapEmployeeDetailsDispatchToProps = (dispatch: Dispatch<any>) => ({
    onAvatarClicked: (employee: Employee) => dispatch(NavigationActions.navigate({
        routeName: 'CurrentProfile',
        params: {employee},
    }))   
});