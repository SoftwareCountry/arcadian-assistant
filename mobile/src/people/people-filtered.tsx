import React from 'react';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LoadingView } from '../navigation/loading';
import { PeopleCompany } from './people-company';
import { PeopleRoom } from './people-room';
import { PeopleDepartment } from './people-department';
import { filterEmployees } from '../reducers/search/search.epics';
import {Map, is} from 'immutable';

interface PeopleProps {
    employees: EmployeesStore;
    loaded: boolean;
}

const mapStateToProps = (state: AppState): PeopleProps => ({
    employees: filterEmployees(state.organization.employees, state.people.filter),
    loaded: state.people.departments && state.people.departments.length > 0,
});

class PeopleCompanyFilteredImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        const su = shouldUpdate(this.props, nextProps);
        return su || this.props.loaded !== nextProps.loaded;
    }

    public render() {
        return !this.props.loaded || !this.props.employees ? <LoadingView/> :
            <PeopleCompany employees={this.props.employees}/>;
    }
}

class PeopleRoomFilteredImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return shouldUpdate(this.props, nextProps);
    }

    public render() {
        return !this.props.employees ? <LoadingView/> : <PeopleRoom employees={this.props.employees}/>;
    }
}

class PeopleDepartmentFilteredImpl extends React.Component<PeopleProps> {
    public shouldComponentUpdate(nextProps: PeopleProps) {
        return shouldUpdate(this.props, nextProps);
    }

    public render() {
        return !this.props.employees ? <LoadingView/> : <PeopleDepartment employees={this.props.employees}/>;
    }
}

function shouldUpdate(curProps: PeopleProps, nextProps: PeopleProps) {
    const somethingUndefined = !curProps.employees || !nextProps.employees;
    const arrays = !is(curProps.employees.employeesById, nextProps.employees.employeesById);
    return somethingUndefined || !somethingUndefined && arrays;
}

export const PeopleCompanyFiltered = connect(mapStateToProps)(PeopleCompanyFilteredImpl);
export const PeopleRoomFiltered = connect(mapStateToProps)(PeopleRoomFilteredImpl);
export const PeopleDepartmentFiltered = connect(mapStateToProps)(PeopleDepartmentFilteredImpl);
