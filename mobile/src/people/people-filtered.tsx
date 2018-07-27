import React from 'react';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LoadingView } from '../navigation/loading';
import { PeopleCompany } from './people-company';
import { PeopleRoom } from './people-room';
import { PeopleDepartment } from './people-department';
import { filterEmployees } from '../reducers/search/search.epics';

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
        return shouldUpdate(this.props, nextProps) || this.props.loaded !== nextProps.loaded;
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
    return !curProps.employees || !nextProps.employees || 
            curProps.employees && nextProps.employees && 
            !curProps.employees.employeesById.equals(nextProps.employees.employeesById);
}

export const PeopleCompanyFiltered = connect(mapStateToProps)(PeopleCompanyFilteredImpl);
export const PeopleRoomFiltered = connect(mapStateToProps)(PeopleRoomFilteredImpl);
export const PeopleDepartmentFiltered = connect(mapStateToProps)(PeopleDepartmentFilteredImpl);
