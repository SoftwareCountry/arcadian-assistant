import React from 'react';
import { connect, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LoadingView } from '../navigation/loading';
import { PeopleCompany } from './people-company';
import { PeopleRoom } from './people-room';
import { PeopleDepartment } from './people-department';
import { startSearch } from '../reducers/search/search.action';
import { SearchType } from '../navigation/search-view';
import { filterEmployees } from '../reducers/search/search.epics';

interface PeopleProps {
    employees: EmployeesStore;
    loaded: boolean;
}
//FILTER!!!!
const mapStateToProps = (state: AppState): PeopleProps => {
    const filteredEmployees = filterEmployees(state.organization.employees, state.people.filter);
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
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return this.props.loaded !== nextProps.loaded || 
            !this.props.employees || this.props.employees && nextProps.employees && 
            !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        return !this.props.loaded || !this.props.employees ? <LoadingView/> :
            <PeopleCompany employees={this.props.employees}/>;
    }
}

class PeopleRoomFilteredImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return !this.props.employees || this.props.employees && nextProps.employees && 
            !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        return !this.props.employees ? <LoadingView/> : <PeopleRoom employees={this.props.employees}/>;
    }
}

class PeopleDepartmentFilteredImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return !this.props.employees || this.props.employees && nextProps.employees && 
            !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        return !this.props.employees ? <LoadingView/> : <PeopleDepartment employees={this.props.employees}/>;
    }
}

export const PeopleCompanyFiltered = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyFilteredImpl);
export const PeopleRoomFiltered = connect(mapStateToProps)(PeopleRoomFilteredImpl);
export const PeopleDepartmentFiltered = connect(mapStateToProps)(PeopleDepartmentFilteredImpl);
