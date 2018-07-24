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
import { updateLeaves } from '../reducers/people/people.reducer';
import { employeesAZComparer } from './employee-comparer';

interface PeopleCompanySearchOwnProps {
    employees: EmployeesStore;
}

interface PeopleCompanyStateProps {
    routeName: string;
    departments: Department[];
    departmentsBranch: Department[];
    employee: Employee;
    departmentLists: DepartmentsListStateDescriptor[];
    employeesPredicate: (head: Department, employee: Employee) => boolean;
}

const mapStateToProps: MapStateToProps<PeopleCompanySearchProps, PeopleCompanySearchOwnProps, AppState> = (state: AppState, ownProps): PeopleCompanySearchProps => ({
    employees: ownProps.employees, // own props

    routeName: 'Company', 
    departments: state.people.filteredDepartments,
    departmentsBranch: state.people.departmentsBranch,
    employee: state.organization.employees.employeesById.get(state.userInfo.employeeId),
    departmentLists: state.people.departmentsLists,
    employeesPredicate: (head: Department, employee: Employee) => employee.departmentId === head.departmentId && employee.employeeId !== head.chiefId,
});

type PeopleCompanySearchProps = PeopleCompanySearchOwnProps & PeopleCompanyStateProps;

interface PeopleCompanyDispatchProps {
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentsBranch: (deps: Department[], depLists: DepartmentsListStateDescriptor[], filteredDeps: Department[]) => void;
    onItemClicked: (employee: Employee) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    requestEmployeesForDepartment: (departmentId: string) => { 
        dispatch(loadEmployeesForDepartment(departmentId)); 
    },
    updateDepartmentsBranch: (deps: Department[], depLists: DepartmentsListStateDescriptor[], filteredDeps: Department[]) => { 
        dispatch(updateDepartmentsBranch(deps, depLists, filteredDeps)); 
    },
    onItemClicked: (employee: Employee) => {
        if (employee) {
            dispatch(openEmployeeDetailsAction(employee));
        }
    },
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanySearchProps & PeopleCompanyDispatchProps> {
    private listDepartments: JSX.Element[];

    constructor(props: PeopleCompanySearchProps & PeopleCompanyDispatchProps) {
        super(props);
        this.listDepartments = renderDepartments(this.props);
    }

    public componentWillUpdate() {
        this.listDepartments = renderDepartments(this.props);
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

        return <ScrollView style={styles.company}>
                    <EmployeeCardWithAvatar
                        employee={chief}
                        departmentAbbreviation={this.props.departmentsBranch[0].abbreviation}
                        onItemClicked = {this.props.onItemClicked}
                    />
                    {this.listDepartments}
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
}

function renderDepartments(props: PeopleCompanySearchProps & PeopleCompanyDispatchProps) {
    const update = (departmentId: string, treeLevel: number) => {
        const res = updateLeaves(props.departmentsBranch, props.departmentLists,
                                 treeLevel, departmentId, props.departments);
        props.updateDepartmentsBranch(res.departmentsLineup, res.departmentsLists, props.departments);
    };

    return props.departmentsBranch.map((head, index) => {
        return <DepartmentsHScrollableList
                departments={props.departments.filter(department => department.parentDepartmentId === head.departmentId)}
                departmentsLists={props.departmentLists[index + 1]}
                headDepartment={head}
                employees={props.employees}
                updateDepartmentsBranch={(depId: string) => update(depId, index + 1)}
                onItemClicked={props.onItemClicked}
                key={head.departmentId}
                requestEmployeesForDepartment={props.requestEmployeesForDepartment}
            />;
    });
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
