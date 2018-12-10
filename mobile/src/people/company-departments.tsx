import React, { Component } from 'react';
import { CompanyDepartmentsLevel } from './company-departments-level';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { rootId } from '../reducers/people/append-root';
import {
    DepartmentIdToChildren,
    DepartmentIdToNode,
    DepartmentIdToSelectedId,
    DepartmentNode
} from '../reducers/people/people.model';
import { selectCompanyDepartment } from '../reducers/people/people.action';
import { EmployeeIdsGroupMap, EmployeeMap } from '../reducers/organization/employees.reducer';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { Employee } from '../reducers/organization/employee.model';
import { ScrollView } from 'react-navigation';
import { LoadingView } from '../navigation/loading';
import { Action, Dispatch } from 'redux';
import { openEmployeeDetails } from '../navigation/navigation.actions';
import { Nullable } from 'types';

//============================================================================
interface CompanyDepartmentsStateProps {
    departmentIdToNode: Nullable<DepartmentIdToNode>;
    headDepartment: Nullable<DepartmentNode>;
    filter: string;
    selectedCompanyDepartmentId: Nullable<string>;
    employeesById: Nullable<EmployeeMap>;
    employeeIdsByDepartment: Nullable<EmployeeIdsGroupMap>;
}

//============================================================================
interface CompanyDepartmentsDispatchProps {
    selectCompanyDepartment: (departmentId: string) => void;
    loadEmployeesForDepartment: (departmentId: string) => void;
    onPressEmployee: (Employee: Employee) => void;
}


//============================================================================
type CompanyDepartmentsProps = CompanyDepartmentsStateProps & CompanyDepartmentsDispatchProps;

//============================================================================
class CompanyDepartmentsImpl extends Component<CompanyDepartmentsProps> {
    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: CompanyDepartmentsProps) {
        return (
            !this.areNodesEqual(this.props.departmentIdToNode, nextProps.departmentIdToNode) ||
            !this.isHeadDepartmentSame(this.props.headDepartment, nextProps.headDepartment) ||
            this.props.filter !== nextProps.filter ||
            this.props.selectedCompanyDepartmentId !== nextProps.selectedCompanyDepartmentId ||
            !this.areEmployeesEqual(this.props.employeesById, nextProps.employeesById) ||
            !this.areEmployeeIdsGroupMapsEqual(this.props.employeeIdsByDepartment, nextProps.employeeIdsByDepartment)
        );
    }

    //----------------------------------------------------------------------------
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

    //----------------------------------------------------------------------------
    private buildData() {
        const employeesById = this.filterEmployees();

        let departmentIdToNode = this.props.departmentIdToNode;

        if (this.props.filter) {
            const filteredDepartmentsNodes = this.filterDepartments(departmentIdToNode, employeesById);
            departmentIdToNode = this.buildBranchFromChildToParent(departmentIdToNode, filteredDepartmentsNodes);
        }

        const departmentIdToChildren = this.buildDepartmentIdToChildren(departmentIdToNode);
        const selectedCompanyDepartmentId = this.buildSelectedDepartmentId(departmentIdToNode, employeesById);

        const selection = selectedCompanyDepartmentId
            ? this.buildDepartmentsSelection(this.props.departmentIdToNode, selectedCompanyDepartmentId)
            : {};

        return {
            employeesById: employeesById,
            employeeIdsByDepartment: this.props.employeeIdsByDepartment,
            departmentIdToChildren: departmentIdToChildren,
            selection: selection
        };
    }

    //----------------------------------------------------------------------------
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

    //----------------------------------------------------------------------------
    private filterDepartments(departmentIdToNode: Nullable<DepartmentIdToNode>,
                              employeesById: Nullable<EmployeeMap>): DepartmentIdToNode {
        if (!departmentIdToNode || !employeesById) {
            return new Map();
        }

        const employees = employeesById.toIndexedSeq().toArray();
        const array = Array.from(departmentIdToNode.entries());

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

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private buildBranchFromChildToParent(departmentIdToNode: Nullable<DepartmentIdToNode>,
                                         filteredDepartmentNodes: DepartmentIdToNode): DepartmentIdToNode {

        const newDepartmentIdsToNodes: DepartmentIdToNode = new Map();

        if (!departmentIdToNode) {
            return newDepartmentIdsToNodes;
        }

        for (let [, departmentNode] of filteredDepartmentNodes.entries()) {
            const node = departmentIdToNode.get(departmentNode.departmentId);
            if (node) {-
                newDepartmentIdsToNodes.set(departmentNode.departmentId, node);
            }

            let parentDepartment = departmentNode.parentId ? departmentIdToNode.get(departmentNode.parentId) : null;

            while (parentDepartment) {
                newDepartmentIdsToNodes.set(parentDepartment.departmentId, parentDepartment);
                parentDepartment = parentDepartment.parentId ? departmentIdToNode.get(parentDepartment.parentId) : null;
            }
        }

        return newDepartmentIdsToNodes;
    }

    //----------------------------------------------------------------------------
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
                const employeesIds = this.props.employeeIdsByDepartment ?
                    this.props.employeeIdsByDepartment.get(node.parentId) :
                    null;
                const parent = departmentIdToNode.get(node.parentId);

                if (!employeesIds || !employeesIds.size || !parent ||
                    (employeesIds.size === 1 && parent && parent.chiefId && employeesIds.has(parent.chiefId))) {
                    continue;
                }

                node.abbreviation = `${parent.abbreviation} Staff`;
            }

            children[node.parentId].push(node);
        }

        return children;
    }

    //----------------------------------------------------------------------------
    private buildSelectedDepartmentId(departmentIdToNode: Nullable<DepartmentIdToNode>,
                                      employeesById: Nullable<EmployeeMap>): Nullable<string> {
        if (!departmentIdToNode || !this.props.selectedCompanyDepartmentId) {
            return null;
        }

        if (departmentIdToNode.has(this.props.selectedCompanyDepartmentId)) {
            return this.props.selectedCompanyDepartmentId;
        }

        const firstEmployee = employeesById ? employeesById.first(null) : null;

        return firstEmployee
            ? firstEmployee.departmentId
            : null;
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private buildDepartmentsSelection(departmentIdToNode: Nullable<DepartmentIdToNode>,
                                      selectedDepartmentId: Nullable<string>): DepartmentIdToSelectedId {

        const departmentIdToSelectedId: DepartmentIdToSelectedId = {};
        if (!departmentIdToNode || !selectedDepartmentId) {
            return departmentIdToSelectedId;
        }

        let selectedDepartment = departmentIdToNode.get(selectedDepartmentId);
        let parent = selectedDepartment && selectedDepartment.parentId ?
            departmentIdToNode.get(selectedDepartment.parentId) :
            null;

        while (parent) {
            if (selectedDepartment) {
                departmentIdToSelectedId[parent.departmentId] = selectedDepartment.departmentId;
            }

            selectedDepartment = departmentIdToNode.get(parent.departmentId);
            parent = selectedDepartment && selectedDepartment.parentId ?
                departmentIdToNode.get(selectedDepartment.parentId) :
                null;
        }

        return departmentIdToSelectedId;
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private isHeadDepartmentSame(a: Nullable<DepartmentNode>, b: Nullable<DepartmentNode>): boolean {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        return a.equals(b);
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
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
            const x = b.get(node.departmentId);
            if (!node.equals(x ? x : null)) {
                return false;
            }
        }

        return true;
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
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

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
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

//----------------------------------------------------------------------------
const stateToProps = (state: AppState) => ({
    departmentIdToNode: state.people ? state.people.departmentIdToNodes : null,
    headDepartment: state.people ? state.people.headDepartment : null,
    filter: state.people ? state.people.filter : '',
    selectedCompanyDepartmentId: state.people ? state.people.selectedCompanyDepartmentId : null,
    employeesById: state.organization ? state.organization.employees.employeesById : null,
    employeeIdsByDepartment: state.organization ? state.organization.employees.employeeIdsByDepartment : null,
});

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>) => ({
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

export const CompanyDepartments = connect(stateToProps, dispatchToProps)(CompanyDepartmentsImpl);
