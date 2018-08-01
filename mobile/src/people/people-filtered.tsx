import React from 'react';
import { connect } from 'react-redux';

import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LoadingView } from '../navigation/loading';
import { PeopleCompany } from './people-company';
import { PeopleRoom } from './people-room';
import { PeopleDepartment } from './people-department';
import { Map } from 'immutable';

interface PeopleProps {
    loaded: boolean;
    employees: EmployeesStore;
}

const mapStateToProps = (state: AppState): PeopleProps => {
    const filter = state.people.filter;
    const employees = state.organization.employees;  
    const employeesPredicate = (employee: Employee) => {
        return (employee.name && employee.name.includes(filter) ||
                employee.email && employee.email.includes(filter) || 
                employee.position && employee.position.includes(filter)
        );
    };
    const filteredEmployeesById: Map<string, Employee> = employees.employeesById.filter(employeesPredicate) as Map<string, Employee>;
    const filteredEmployees : EmployeesStore = {
        ...employees,
        employeesById: filteredEmployeesById, 
    };


    return ({
        loaded: state.people.departmentsBranch.length > 0 && 
                state.people.departments && state.people.departments.length > 0,
        employees: filteredEmployees,
    });
};

class PeopleCompanyImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return this.props.loaded !== nextProps.loaded || !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        if (!this.props.loaded) {
            return <LoadingView/>;
        }

        return <PeopleCompany employees={this.props.employees}/>;
    }
}

class PeopleRoomImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        return <PeopleRoom employees={this.props.employees}/>;
    }
}

class PeopleDepartmentImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        return <PeopleDepartment employees={this.props.employees}/>;
    }
}

export const PeopleCompanyFiltered = connect(mapStateToProps)(PeopleCompanyImpl);
export const PeopleRoomFiltered = connect(mapStateToProps)(PeopleRoomImpl);
export const PeopleDepartmentFiltered = connect(mapStateToProps)(PeopleDepartmentImpl);
