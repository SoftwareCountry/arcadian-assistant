import React from 'react';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LoadingView } from '../navigation/loading';
import { PeopleRoom } from './people-room';
import { PeopleDepartment } from './people-department';
import { is, Map, Set } from 'immutable';
import { CompanyDepartments } from './company-departments';
import { Action, Dispatch } from 'redux';
import { NavigationScreenProps } from 'react-navigation';
import { loadAllEmployees } from '../reducers/organization/organization.action';
import { Employee } from '../reducers/organization/employee.model';

//============================================================================
interface PeopleProps {
    employees: EmployeesStore;
    loaded: boolean;
}

const mapStateToProps = (state: AppState): PeopleProps => ({
    employees: state.organization && state.people ? filterEmployees(state.organization.employees, state.people.filter) : {
        employeesById: Map(),
        employeeIdsByDepartment: Map()
    },
    loaded: !!state.organization && state.organization.departments && state.organization.departments.length > 0 && !state.organization.employees.employeesById.isEmpty(),
});

//============================================================================
interface PeopleCompanyDispatchProps {
    loadAllEmployees: () => void;
}

//============================================================================
class PeopleCompanyFilteredImpl extends React.Component<PeopleProps & PeopleCompanyDispatchProps & NavigationScreenProps> {

    //----------------------------------------------------------------------------
    public componentDidMount() {
        this.props.loadAllEmployees();
    }

    //----------------------------------------------------------------------------
    public render() {
        return !this.props.loaded ? <LoadingView/> : <CompanyDepartments/>;
    }
}

//----------------------------------------------------------------------------
const companyDispatchToProps = (dispatch: Dispatch<Action>): PeopleCompanyDispatchProps => ({
    loadAllEmployees: () => dispatch(loadAllEmployees()),
});

//============================================================================
class PeopleRoomFilteredImpl extends React.Component<PeopleProps> {

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return shouldUpdate(this.props, nextProps);
    }

    //----------------------------------------------------------------------------
    public render() {
        return !this.props.loaded ? <LoadingView/> : <PeopleRoom employees={this.props.employees}/>;
    }
}

//============================================================================
class PeopleDepartmentFilteredImpl extends React.Component<PeopleProps> {

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return shouldUpdate(this.props, nextProps);
    }

    //----------------------------------------------------------------------------
    public render() {
        return !this.props.loaded ? <LoadingView/> : <PeopleDepartment employees={this.props.employees}/>;
    }
}

//----------------------------------------------------------------------------
function shouldUpdate(curProps: PeopleProps, nextProps: PeopleProps) {
    const somethingUndefined = !curProps.employees || !nextProps.employees;
    const arrays = !is(curProps.employees.employeesById, nextProps.employees.employeesById);
    return somethingUndefined || !somethingUndefined && arrays;
}

//----------------------------------------------------------------------------
function filterEmployees(employees: EmployeesStore, filter: string) {

    const lowerCasedFilter = filter.toLowerCase();

    const employeesPredicate = (employee: Employee) => {
        const name = employee.getName().toLowerCase();
        const surname = employee.getSurname().toLowerCase();
        const position = employee.position.toLowerCase();

        return surname.startsWith(lowerCasedFilter) ||
            name.startsWith(lowerCasedFilter) ||
            position.startsWith(lowerCasedFilter);
    };

    // filter employees
    const filteredEmployeesById: Map<string, Employee> = employees.employeesById.filter(employeesPredicate) as Map<string, Employee>;
    let filteredEmployeesByDep: Map<string, Set<string>> =
        employees.employeeIdsByDepartment.map(d => d.filter(e => filteredEmployeesById.has(e))) as Map<string, Set<string>>;
    // clear empty departments
    filteredEmployeesByDep = filteredEmployeesByDep.filter(e => !e.isEmpty()) as Map<string, Set<string>>;
    return { employeesById: filteredEmployeesById, employeeIdsByDepartment: filteredEmployeesByDep };
}

export const PeopleCompanyFiltered = connect(mapStateToProps, companyDispatchToProps)(PeopleCompanyFilteredImpl);
export const PeopleRoomFiltered = connect(mapStateToProps)(PeopleRoomFilteredImpl);
export const PeopleDepartmentFiltered = connect(mapStateToProps)(PeopleDepartmentFilteredImpl);
