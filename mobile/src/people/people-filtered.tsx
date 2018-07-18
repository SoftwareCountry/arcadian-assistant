import React from 'react';
import { connect } from 'react-redux';

import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LoadingView } from '../navigation/loading';
import { PeopleCompany } from './people-company';
import { PeopleRoom } from './people-room';
import { PeopleDepartment } from './people-department';
import { Map, Set } from 'immutable';
import { Department } from '../reducers/organization/department.model';

interface PeopleProps {
    loaded: boolean;
    employees: EmployeesStore;
    departments: Department[];
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
    // filter employees
    const filteredEmployeesById: Map<string, Employee> = employees.employeesById.filter(employeesPredicate) as Map<string, Employee>;
    let filteredEmployeesByDep: Map<string, Set<string>> = 
        employees.employeeIdsByDepartment.map(d => d.filter(e => filteredEmployeesById.has(e))) as Map<string, Set<string>>;
    // clear empty departments
    filteredEmployeesByDep = filteredEmployeesByDep.filter(e => !e.isEmpty()) as Map<string, Set<string>>;
    const filteredEmployees : EmployeesStore = {employeesById: filteredEmployeesById, 
                                                employeeIdsByDepartment: filteredEmployeesByDep};

    return ({
        loaded: state.people.departmentsBranch.length > 0 && 
                state.people.departments && state.people.departments.length > 0,
        employees: filteredEmployees,
        departments: state.people.departments,
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

        const filteredDeps = this.props.departments.filter((d) => this.props.employees.employeeIdsByDepartment.has(d.departmentId));
        let deps = filteredDeps;
        filteredDeps.forEach(dep => {
            let curDep = dep;
            while (!curDep.isHeadDepartment) {
                curDep = this.props.departments.filter(d => d.departmentId === curDep.parentDepartmentId)[0];
                if (deps.findIndex(e => e === curDep) === -1) {
                    deps.push(curDep);
                }
            }
        });
        return <PeopleCompany employees={this.props.employees} departments={deps}/>;
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
