import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { View } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap, EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { SearchPeopleView } from '../navigation/search-view';

interface PeopleRoomProps {
    employees: EmployeesStore;
    userEmployee: Employee;
    filter: string;
}

const mapStateToProps = (state: AppState): PeopleRoomProps => ({
    employees: state.organization.employees,
    userEmployee: state.organization.employees.employeesById.get(state.userInfo.employeeId),
    filter: state.people.filter,
});

interface EmployeesListDispatchProps {
    onItemClicked: (employee: Employee) => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeesListDispatchProps => ({
    onItemClicked: (employee: Employee) => dispatch(openEmployeeDetailsAction(employee))
});

export class PeopleRoomImpl extends React.Component<PeopleRoomProps & EmployeesListDispatchProps> {
    public shouldComponentUpdate(nextProps: PeopleRoomProps & EmployeesListDispatchProps) {
        if (this.props.onItemClicked !== nextProps.onItemClicked
            || this.props.userEmployee !== nextProps.userEmployee
        ) {
            return true;
        }

        const employees = this.props.employees.employeesById.filter(this.employeesPredicate);
        const nextEmployees = nextProps.employees.employeesById.filter(this.employeesPredicate);

        if (!employees.equals(nextEmployees)) {
            return true;
        } else {
            return false;
        }
    }

    public render() {
        const employees = this.props.employees.employeesById.toArray().filter(this.employeesPredicate);

        return <View>
                <SearchPeopleView/>
                <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked}/>
            </View>;
    }

    private employeesPredicate = (employee: Employee) => {
        return employee.name.includes(this.props.filter) && 
            this.props.userEmployee && employee.roomNumber === this.props.userEmployee.roomNumber;
    }
}

export const PeopleRoom = connect(mapStateToProps, mapDispatchToProps)(PeopleRoomImpl);