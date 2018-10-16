import React from 'react';
import { connect, Dispatch, MapStateToProps } from 'react-redux';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';

interface PeopleRoomPropsOwnProps {
    employees: EmployeesStore;
    customEmployeesPredicate?: (employee: Employee) => boolean;
}

interface PeopleRoomStateProps {
    employees: EmployeesStore;
    userEmployee: Employee;
    employeesPredicate: (employee: Employee) => boolean;
}

type PeopleDepartmentProps = PeopleRoomStateProps & PeopleRoomPropsOwnProps;

const mapStateToProps: MapStateToProps<PeopleRoomStateProps, PeopleRoomPropsOwnProps, AppState> = 
    (state: AppState, ownProps: PeopleRoomPropsOwnProps): PeopleRoomStateProps => {
        const userEmployee = state.organization.employees.employeesById.get(state.userInfo.employeeId);
        const defaultEmployeesPredicate = (employee: Employee) => userEmployee && employee.roomNumber === userEmployee.roomNumber;

        return ({
            employees: ownProps.employees,
            userEmployee,
            employeesPredicate: ownProps.customEmployeesPredicate ? ownProps.customEmployeesPredicate : defaultEmployeesPredicate
        });
    };

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee))
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
        const employees = this.props.employees.employeesById.toArray().filter(this.props.employeesPredicate);

        return <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked}/>;
    }
}

export const PeopleRoom = connect(mapStateToProps, mapDispatchToProps)(PeopleRoomImpl);