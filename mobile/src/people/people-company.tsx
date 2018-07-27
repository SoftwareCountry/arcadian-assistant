import React from 'react';
import { connect, Dispatch, MapStateToProps } from 'react-redux';
import { ScrollView } from 'react-native';

import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { DepartmentsHScrollableList, DepartmentsListStateDescriptor } from './departments/departments-horizontal-scrollable-list';
import { EmployeeCardWithAvatar } from './departments/employee-card-with-avatar';
import { EmployeeCardWithAvatarList } from './departments/employee-card-with-avatar-list';
import { PeopleActions, updateDepartmentsBranch } from '../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';
import { Employee } from '../reducers/organization/employee.model';
import { employeesListStyles as styles } from './styles';
import { EmployeesStore } from '../reducers/organization/employees.reducer';
import { departmentsBranchFromDepartmentWithId } from '../reducers/people/people.reducer';
import { employeesAZComparer } from './employee-comparer';
import { recountBranch, recountDepartments } from '../reducers/search/search.epics';
import { filterDepartmentsFinished } from '../reducers/search/search.action';

interface PeopleCompanySearchOwnProps {
    employees: EmployeesStore;
}

interface PeopleCompanyStateProps {
    routeName: string;
    departments: Department[];
    filteredDepartments: Department[];
    departmentsBranch: Department[];
    employee: Employee;
    departmentLists: DepartmentsListStateDescriptor[];
    employeesPredicate: (head: Department, employee: Employee) => boolean;
}

const mapStateToProps: MapStateToProps<PeopleCompanySearchProps, PeopleCompanySearchOwnProps, AppState> = (state: AppState, ownProps): PeopleCompanySearchProps => ({
    employees: ownProps.employees, // own props

    routeName: 'Company', 
    departments: state.people.departments,
    filteredDepartments: state.people.filteredDepartments,
    departmentsBranch: state.people.departmentsBranch,
    employee: state.organization.employees.employeesById.get(state.userInfo.employeeId),
    departmentLists: state.people.departmentsLists,
    employeesPredicate: (head: Department, employee: Employee) => employee.departmentId === head.departmentId && employee.employeeId !== head.chiefId,
});

type PeopleCompanySearchProps = PeopleCompanySearchOwnProps & PeopleCompanyStateProps;

interface PeopleCompanyDispatchProps {
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentsBranch: (deps: Department[], depId: string) => void;
    onItemClicked: (employee: Employee) => void;
    filterDepartmentsFinished: (filteredDeps: Department[], departmentBranch: Department[],
                                departmentList: DepartmentsListStateDescriptor[]) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    requestEmployeesForDepartment: (departmentId: string) => { 
        dispatch(loadEmployeesForDepartment(departmentId)); 
    },
    updateDepartmentsBranch: (deps: Department[], depId: string) => { 
        dispatch(updateDepartmentsBranch(deps, depId)); 
    },
    onItemClicked: (employee: Employee) => {
        if (employee) {
            dispatch(openEmployeeDetailsAction(employee));
        }
    },
    filterDepartmentsFinished: (filteredDeps: Department[], departmentBranch: Department[],
                                departmentList: DepartmentsListStateDescriptor[]) => {
        dispatch(filterDepartmentsFinished(filteredDeps, departmentBranch, departmentList));
    },
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanySearchProps & PeopleCompanyDispatchProps> {
    public componentWillMount() {
        // recalculate current branch
        const deps = recountDepartments(this.props.departments, this.props.employees);
        const newBranch = recountBranch(this.props.departments, this.props.departmentsBranch, deps);
        this.props.filterDepartmentsFinished(deps, newBranch.departmentsLineup, newBranch.departmentsLists);
    }

    public componentWillUpdate(nextProps: PeopleCompanySearchProps & PeopleCompanyDispatchProps) {
        if (nextProps.departmentsBranch && nextProps.departmentsBranch.length > 0) {
            const lowestLevel = nextProps.departmentsBranch[nextProps.departmentsBranch.length - 1];
            nextProps.requestEmployeesForDepartment(lowestLevel.departmentId);
        }
    }

    public render() {
        if (!this.props.departmentsBranch || this.props.departmentsBranch.length <= 0) {
            return null;
        }
        
        const chief = this.props.employees.employeesById.get(this.props.departmentsBranch[0].chiefId);
        //list of employees
        const lowestLevel = this.props.departmentsBranch[this.props.departmentsBranch.length - 1];
        const employeesPredicate = (e: Employee) => this.props.employeesPredicate(lowestLevel, e);
        const subordinates = this.props.employees.employeesById.filter(employeesPredicate).toArray();
        //list of departments 
        const departments = this.renderDepartments();

        return <ScrollView style={styles.company}>
                    <EmployeeCardWithAvatar
                        employee={chief}
                        departmentAbbreviation={this.props.departmentsBranch[0].abbreviation}
                        onItemClicked = {this.props.onItemClicked}
                    />
                    {departments}
                    {
                        (subordinates != null && subordinates.length > 0) ? 
                        <EmployeeCardWithAvatarList 
                            employees={subordinates.sort(employeesAZComparer)} 
                            treeLevel={this.props.departmentsBranch.length - 1}
                            onItemClicked={this.props.onItemClicked}
                        /> : null
                    }
                </ScrollView>;
    }

    private update = (departmentId: string, treeLevel: number) => {
        this.props.updateDepartmentsBranch(this.props.filteredDepartments, departmentId);
        this.props.requestEmployeesForDepartment(departmentId);
    }

    private renderDepartments = () => {  
        return this.props.departmentsBranch.map((head, index) => {
            return <DepartmentsHScrollableList
                        departments={this.props.filteredDepartments.filter(department => department.parentDepartmentId === head.departmentId)}
                        departmentsLists={this.props.departmentLists[index + 1]}
                        headDepartment={head}
                        employees={this.props.employees}
                        updateDepartmentsBranch={(depId: string) => this.update(depId, index + 1)}
                        onItemClicked={this.props.onItemClicked}
                        key={head.departmentId}
                    />;
        });
    }
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
