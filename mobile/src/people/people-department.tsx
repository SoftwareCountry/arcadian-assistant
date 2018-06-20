import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { View } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { EmployeeMap, EmployeesStore } from '../reducers/organization/employees.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { employeesAZSort } from './employee-comparer';
import { SearchPeopleView } from '../navigation/search-view';

interface PeopleDepartmentProps {
    employees: EmployeesStore;
    userEmployee: Employee;
    filter: string;
}

const mapStateToProps = (state: AppState): PeopleDepartmentProps => ({
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

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps & EmployeesListDispatchProps> {
    public shouldComponentUpdate(nextProps: PeopleDepartmentProps & EmployeesListDispatchProps) {
        if (this.props.onItemClicked !== nextProps.onItemClicked
            || this.props.userEmployee !== nextProps.userEmployee
        ) {
            return true;
        }

        const employees = this.props.employees.employeesById.filter((e) => PeopleDepartmentImpl.employeesPredicate(e, this.props));
        const nextEmployees = nextProps.employees.employeesById.filter((e) => PeopleDepartmentImpl.employeesPredicate(e, nextProps));

        if (!employees.equals(nextEmployees)) {
            return true;
        } else {
            return false;
        }
    }

    public render() {
        const employees = this.props.employees.employeesById.toArray().filter((e) => PeopleDepartmentImpl.employeesPredicate(e, this.props));

        return <View>
            <SearchPeopleView/>
            <EmployeesList employees={employees} onItemClicked={this.props.onItemClicked} />
        </View>;
    }

    private static employeesPredicate = (employee: Employee, props: PeopleDepartmentProps) => {
        return employee.name.includes(props.filter) &&
            props.userEmployee && employee.departmentId === props.userEmployee.departmentId;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);