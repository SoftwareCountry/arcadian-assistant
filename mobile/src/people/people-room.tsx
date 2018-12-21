import React from 'react';
import { connect } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { LoadingView } from '../navigation/loading';
import { Action, Dispatch } from 'redux';
import { openEmployeeDetails } from '../navigation/navigation.actions';
import { Optional } from 'types';
import { getEmployee } from '../utils/utils';

interface PeopleRoomPropsOwnProps {
    employees: EmployeesStore;
    customEmployeesPredicate?: (employee: Employee) => boolean;
}

interface PeopleRoomStateProps {
    isLoading: boolean;
    employees: EmployeesStore;
    userEmployee: Optional<Employee>;
    employeesPredicate: (employee: Employee) => boolean;
}

const mapStateToProps = (state: AppState, ownProps: PeopleRoomPropsOwnProps): PeopleRoomStateProps => {
    const userEmployee = getEmployee(state);
    const defaultEmployeesPredicate = (employee: Employee) => !!userEmployee && employee.roomNumber === userEmployee.roomNumber;

    return ({
        isLoading: !state.organization || state.organization.employees.employeesById.isEmpty(),
        employees: ownProps.employees,
        userEmployee,
        employeesPredicate: ownProps.customEmployeesPredicate ? ownProps.customEmployeesPredicate : defaultEmployeesPredicate
    });
};

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<Action>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetails(employee.employeeId))
});

class PeopleRoomImpl extends React.Component<PeopleRoomStateProps & EmployeesListDispatchProps> {
    public shouldComponentUpdate(nextProps: PeopleRoomStateProps & EmployeesListDispatchProps) {
        if (this.props.onItemClicked !== nextProps.onItemClicked
            || this.props.userEmployee !== nextProps.userEmployee
        ) {
            return true;
        }

        const employees = this.props.employees.employeesById.filter(this.props.employeesPredicate);
        const nextEmployees = nextProps.employees.employeesById.filter(nextProps.employeesPredicate);

        return !employees.equals(nextEmployees);
    }

    public render() {
        const employees = this.props.employees.employeesById.toIndexedSeq().toArray().filter(this.props.employeesPredicate);
        if (this.props.isLoading) {
            return <LoadingView/>;
        }
        return <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked}/>;
    }
}

export const PeopleRoom = connect<PeopleRoomStateProps, EmployeesListDispatchProps, PeopleRoomPropsOwnProps, AppState>(mapStateToProps, mapDispatchToProps)(PeopleRoomImpl);
