import React from 'react';
import { connect, Dispatch } from 'react-redux';

import { PeopleActions, updateDepartmentsBranch } from '../reducers/people/people.action';
import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LoadingView } from '../navigation/loading';
import { PeopleCompany } from './people-company';
import { PeopleRoom } from './people-room';
import { PeopleDepartment } from './people-department';
import { Map, Set } from 'immutable';
import { Department } from '../reducers/organization/department.model';
import { DepartmentsListStateDescriptor } from './departments/departments-horizontal-scrollable-list';
import { updateTopOfBranch } from '../reducers/people/people.reducer';

interface PeopleProps {
    employees: EmployeesStore;
}

interface PeopleCompanyFilteredProps {
    loaded: boolean;
    departments: Department[];
    departmentBranch: Department[];
    departmentLists: DepartmentsListStateDescriptor[];
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
        employees: filteredEmployees,
    });
};

const mapStateToCompanyFilteredProps = (state: AppState): PeopleProps & PeopleCompanyFilteredProps => {
    const empl = mapStateToProps(state);
    const branch = state.people.departmentsBranch;

    // filter departments
    const filteredDeps = state.people.departments.filter((d) => empl.employees.employeeIdsByDepartment.has(d.departmentId));
    let deps = filteredDeps;
    filteredDeps.forEach(dep => {
        let curDep = dep;
        while (!curDep.isHeadDepartment) {
            curDep = state.people.departments.filter(d => d.departmentId === curDep.parentDepartmentId)[0];
            if (deps.findIndex(e => e === curDep) === -1) {
                deps.push(curDep);
            }
        }
    });
    // recalculate department branch
    const res = updateTopOfBranch(branch[branch.length - 1].departmentId, deps);

    return ({
        loaded: res.departmentsLineup.length > 0 && 
                state.people.departments && state.people.departments.length > 0,
        employees: empl.employees,
        departments: deps,
        departmentBranch: res.departmentsLineup,
        departmentLists: res.departmentsLists
    });
};

interface PeopleCompanyFilteredDispatchProps {
    updateDepartmentsBranch: (deps: Department[], depLists: DepartmentsListStateDescriptor[]) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    updateDepartmentsBranch: (deps: Department[], depLists: DepartmentsListStateDescriptor[]) => { 
        dispatch(updateDepartmentsBranch(deps, depLists)); 
    },
});

class PeopleCompanyImpl extends React.Component<PeopleProps & PeopleCompanyFilteredProps & PeopleCompanyFilteredDispatchProps> {
    public componentWillMount() {
        this.props.updateDepartmentsBranch(this.props.departmentBranch, this.props.departmentLists);
    }

    public shouldComponentUpdate(nextProps: PeopleProps & PeopleCompanyFilteredProps) {
        if (this.props.loaded !== nextProps.loaded || !this.props.employees.employeesById.equals(nextProps.employees.employeesById) || 
            !this.areEqual(this.props.departmentBranch, nextProps.departmentBranch, (a, b) => a === b) || 
            !this.areEqual(this.props.departmentLists, nextProps.departmentLists, (a, b) => a.currentPage === b.currentPage)) {
                this.props.updateDepartmentsBranch(nextProps.departmentBranch, nextProps.departmentLists);
                return true;
        } 
        return false;
    }

    public render() {
        return !this.props.loaded ? <LoadingView/> :
            <PeopleCompany employees={this.props.employees} departments={this.props.departments}/>;
    }

    private areEqual<T>(first: T[], second: T[], f: (a: T, b: T) => boolean): boolean {
        if (!first || ! second || first.length !== second.length) {
            return false;
        }
        for (let i = 0; i < first.length; i++) {
            if (!f(first[i], second[i])) {
                return false;
            }
        }
        return true;
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

export const PeopleCompanyFiltered = connect(mapStateToCompanyFilteredProps, mapDispatchToProps)(PeopleCompanyImpl);
export const PeopleRoomFiltered = connect(mapStateToProps)(PeopleRoomImpl);
export const PeopleDepartmentFiltered = connect(mapStateToProps)(PeopleDepartmentImpl);
