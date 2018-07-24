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
import { startSearch } from '../reducers/search.action';
import { SearchType } from '../navigation/search-view';

interface PeopleProps {
    employees: EmployeesStore;
    loaded: boolean;
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
        loaded: state.people.departments && state.people.departments.length > 0,
    });
};

interface PeopleFilteredDispatchProps {
    startSearch: () => void;
}
const mapDispatchToProps = (dispatch: Dispatch<any>): PeopleFilteredDispatchProps => ({
    startSearch: () => dispatch(startSearch('', SearchType.People))
});

class PeopleCompanyFilteredImpl extends React.Component<PeopleProps & PeopleFilteredDispatchProps> {
    public componentWillMount() {
        // recalculate current branch if need
        this.props.startSearch();
    }

    public shouldComponentUpdate(nextProps: PeopleProps) {
        return this.props.loaded !== nextProps.loaded || 
            !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        return !this.props.loaded ? <LoadingView/> :
            <PeopleCompany employees={this.props.employees}/>;
    }
}

class PeopleRoomFilteredImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        return <PeopleRoom employees={this.props.employees}/>;
    }
}

class PeopleDepartmentFilteredImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        return <PeopleDepartment employees={this.props.employees}/>;
    }
}

export const PeopleCompanyFiltered = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyFilteredImpl);
export const PeopleRoomFiltered = connect(mapStateToProps)(PeopleRoomFilteredImpl);
export const PeopleDepartmentFiltered = connect(mapStateToProps)(PeopleDepartmentFilteredImpl);
