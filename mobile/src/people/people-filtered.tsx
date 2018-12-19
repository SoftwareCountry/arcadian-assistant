import React from 'react';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { LoadingView } from '../navigation/loading';
import { PeopleRoom } from './people-room';
import { PeopleDepartment } from './people-department';
import { filterEmployees } from '../reducers/search/search.epics';
import { is, Map } from 'immutable';
import { CompanyDepartments } from './company-departments';
import { Action, Dispatch } from 'redux';
import { NavigationEventSubscription, NavigationScreenProps } from 'react-navigation';
import { loadAllEmployees } from '../reducers/organization/organization.action';

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

    private subscription?: NavigationEventSubscription;

    //----------------------------------------------------------------------------
    public componentDidMount() {
        this.subscription = this.props.navigation.addListener('willFocus', () => {
            this.props.loadAllEmployees();
        });
    }

    //----------------------------------------------------------------------------
    public componentWillUnmount(): void {
        if (this.subscription) {
            this.subscription.remove();
        }
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

export const PeopleCompanyFiltered = connect(mapStateToProps, companyDispatchToProps)(PeopleCompanyFilteredImpl);
export const PeopleRoomFiltered = connect(mapStateToProps)(PeopleRoomFilteredImpl);
export const PeopleDepartmentFiltered = connect(mapStateToProps)(PeopleDepartmentFilteredImpl);
