import React, { Component } from 'react';
import { CompanyDepartmentsLevel } from './company-departments-level';
import { connect, MapStateToProps, MapDispatchToProps } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { rootId } from '../reducers/people/append-root';
import { DepartmentIdToChildren, DepartmentIdToSelectedId, DepartmentNode, DepartmentIdToNode } from '../reducers/people/people.model';
import { selectCompanyDepartment } from '../reducers/people/people.action';
import { EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { Employee } from '../reducers/organization/employee.model';
import { ScrollView } from 'react-native';
import { LoadingView } from '../navigation/loading';
import { Action, Dispatch } from 'redux';
import { openEmployeeDetails } from '../navigation/navigation.actions';
import { Nullable, Optional } from 'types';

interface CompanyDepartmentsStateProps {
    departmentIdToNode: Nullable<DepartmentIdToNode>;
    headDepartment: Nullable<DepartmentNode>;
    filter: string;
    selectedCompanyDepartmentId: Nullable<string>;
    employeesById: Nullable<EmployeeMap>;
    employeeIdsByDepartment: Nullable<EmployeeIdsGroupMap>;
}

interface CompanyDepartmentsDispatchProps {
    selectCompanyDepartment: (departmentId: string) => void;
    loadEmployeesForDepartment: (departmentId: string) => void;
    onPressEmployee: (Employee: Employee) => void;
}

const mapStateToProps: MapStateToProps<CompanyDepartmentsStateProps, void, AppState> = (state: AppState) => ({
    departmentIdToNode: state.people ? state.people.departmentIdToNodes : null,
    headDepartment: state.people ? state.people.headDepartment : null,
    filter: state.people ? state.people.filter : '',
    selectedCompanyDepartmentId: state.people ? state.people.selectedCompanyDepartmentId : null,
    employeesById: state.organization ? state.organization.employees.employeesById : null,
    employeeIdsByDepartment: state.organization ? state.organization.employees.employeeIdsByDepartment : null,
});

const mapDispatchToProps: MapDispatchToProps<CompanyDepartmentsDispatchProps, void> = (dispatch: Dispatch<Action>) => ({
    selectCompanyDepartment: (departmentId: string) => {
        dispatch(selectCompanyDepartment(departmentId));
    },
    loadEmployeesForDepartment: (departmentId: string) => {
        dispatch(loadEmployeesForDepartment(departmentId));
    },
    onPressEmployee: (employee: Employee) => {
        dispatch(openEmployeeDetails(employee));
    }
});

type CompanyDepartmentsProps = CompanyDepartmentsStateProps & CompanyDepartmentsDispatchProps;

class CompanyDepartmentsImpl extends Component<CompanyDepartmentsProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsProps) {
        return !this.areNodesEqual(this.props.departmentIdToNode, nextProps.departmentIdToNode)
            || !this.isHeadDepartmentSame(this.props.headDepartment, nextProps.headDepartment)
            || this.props.filter !== nextProps.filter
            || this.props.selectedCompanyDepartmentId !== nextProps.selectedCompanyDepartmentId
            || !this.areEmployeesEqual(this.props.employeesById, nextProps.employeesById)
            || !this.areEmployeeIdsGroupMapsEqual(this.props.employeeIdsByDepartment, nextProps.employeeIdsByDepartment);
    }

    public render() {

        const {
            employeesById,
            employeeIdsByDepartment,
            departmentIdToChildren,
            selection
        } = this.buildData();

        return this.props.headDepartment
            ? (
                <ScrollView overScrollMode={'auto'}>
                    <CompanyDepartmentsLevel
                        departmentId={rootId}
                        departmentIdToChildren={departmentIdToChildren}
                        employeesById={employeesById}
                        employeeIdsByDepartment={employeeIdsByDepartment}
                        selection={selection}
                        onSelectedNode={this.props.selectCompanyDepartment}
                        onPressEmployee={this.props.onPressEmployee}
                        loadEmployeesForDepartment={this.props.loadEmployeesForDepartment}/>
                </ScrollView>
            ) : <LoadingView/>;
    }

    private buildData() {
        const employeesById = this.filterEmployees();

        let departmentIdToNode = this.props.departmentIdToNode;

        if (this.props.filter) {
            const filteredDepartmentsNodes = this.filterDepartments(employeesById);
            departmentIdToNode = this.buildBranchFromChildToParent(filteredDepartmentsNodes);
        }

        const departmentIdToChildren = this.buildDepartmentIdToChildren(departmentIdToNode);

        const selectedCompanyDepartmentId = this.buildSelectedDepartmentId(departmentIdToNode, employeesById);

        const selection = selectedCompanyDepartmentId
            ? this.buildDepartmentsSelection(selectedCompanyDepartmentId)
            : {};

        return {
            employeesById: employeesById,
            employeeIdsByDepartment: this.props.employeeIdsByDepartment,
            departmentIdToChildren: departmentIdToChildren,
            selection: selection
        };
    }

    private filterEmployees(): Nullable<EmployeeMap> {
        if (!this.props.employeesById) {
            return null;
        }

        if (!this.props.filter) {
            return this.props.employeesById;
        }

        return this.props.employeesById
            .filter(employee => employee.name.toLowerCase().includes(this.props.filter.toLowerCase()))
            .toMap();
    }

    private filterDepartments(employeesById: EmployeeMap): DepartmentIdToNode {
        const employees = employeesById.toIndexedSeq().toArray();
        const array = Array.from(this.props.departmentIdToNode.entries());

        const filteredDepartmentNodes = array.filter(([, departmentNode]) => {

            for (let employee of employees) {
                if (departmentNode.staffDepartmentId && departmentNode.staffDepartmentId === employee.departmentId) {
                    return true;
                }

                if (employee.departmentId === departmentNode.departmentId) {
                    return true;
                }
            }

            return false;
        });

        return new Map(filteredDepartmentNodes);
    }

    private buildBranchFromChildToParent(filteredDepartmentNodes: DepartmentIdToNode): DepartmentIdToNode {
        const newDepartmentIdsToNodes: DepartmentIdToNode = new Map();

        for (let [, departmentNode] of filteredDepartmentNodes.entries()) {
            newDepartmentIdsToNodes.set(departmentNode.departmentId, this.props.departmentIdToNode.get(departmentNode.departmentId));

            let parentDepartment = this.props.departmentIdToNode.get(departmentNode.parentId);

            while (parentDepartment) {
                newDepartmentIdsToNodes.set(parentDepartment.departmentId, parentDepartment);
                parentDepartment = this.props.departmentIdToNode.get(parentDepartment.parentId);
            }
        }

        return newDepartmentIdsToNodes;
    }

    private buildDepartmentIdToChildren(departmentIdToNode: Nullable<DepartmentIdToNode>): DepartmentIdToChildren {
        const children: DepartmentIdToChildren = {};

        if (!departmentIdToNode) {
            return children;
        }

        for (let [, node] of departmentIdToNode.entries()) {

            if (!node.parentId) {
                continue;
            }

            if (!children[node.parentId]) {
                children[node.parentId] = [];
            }

            if (node.staffDepartmentId) {
                const employeesIds = this.props.employeeIdsByDepartment.get(node.parentId);
                const parent = departmentIdToNode.get(node.parentId);

                if (!employeesIds
                    || !employeesIds.size
                    || (employeesIds.size === 1 && parent && employeesIds.has(parent.chiefId))) {
                        continue;
                }

                node.abbreviation = `${parent.abbreviation} Staff`;
            }

            children[node.parentId].push(node);
        }

        return children;
    }

    private buildSelectedDepartmentId(departmentIdToNode: Nullable<DepartmentIdToNode>, employeesById: EmployeeMap): Optional<string> {
        if (!departmentIdToNode || !this.props.selectedCompanyDepartmentId) {
            return null;
        }

        if (departmentIdToNode.has(this.props.selectedCompanyDepartmentId)) {
            return this.props.selectedCompanyDepartmentId;
        }

        const firstEmployee = employeesById.first(null);

        return firstEmployee
            ? firstEmployee.departmentId
            : null;
    }

    private buildDepartmentsSelection(selectedDepartmentId: string): DepartmentIdToSelectedId {
        const departmentIdToSelectedId: DepartmentIdToSelectedId = {};
        let selectedDepartment = this.props.departmentIdToNode.get(selectedDepartmentId);
        let parent = selectedDepartment ? this.props.departmentIdToNode.get(selectedDepartment.parentId) : null;

        while (parent) {
            departmentIdToSelectedId[parent.departmentId] = selectedDepartment.departmentId;
            selectedDepartment = this.props.departmentIdToNode.get(parent.departmentId);
            parent = selectedDepartment ? this.props.departmentIdToNode.get(selectedDepartment.parentId) : null;
        }

        return departmentIdToSelectedId;
    }

    private isHeadDepartmentSame(a: Nullable<DepartmentNode>, b: Nullable<DepartmentNode>): boolean {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        return a.equals(b);
    }

    private areNodesEqual(a: Nullable<DepartmentIdToNode>, b: Nullable<DepartmentIdToNode>): boolean {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        if (a.size !== b.size) {
            return false;
        }

        for (let [, node] of a) {
            const x = b.get(node.departmentId)
            if (!node.equals(x ? x : null)) {
                return false;
            }
        }

        return true;
    }

    private areEmployeesEqual(a: Nullable<EmployeeMap>, b: Nullable<EmployeeMap>) {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        if (a.equals(b)) {
            return true;
        }

        const aArray = a.toIndexedSeq().toArray();
        const bArray = b.toIndexedSeq().toArray();

        for (let i = 0; i < aArray.length; i++) {
            if (!aArray[i].equals(bArray[i])) {
                return false;
            }
        }

        return true;
    }

    private areEmployeeIdsGroupMapsEqual(a: Nullable<EmployeeIdsGroupMap>, b: Nullable<EmployeeIdsGroupMap>) {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        if (a.equals(b)) {
            return true;
        }
    }
}

export const CompanyDepartments = connect(mapStateToProps, mapDispatchToProps)(CompanyDepartmentsImpl);
