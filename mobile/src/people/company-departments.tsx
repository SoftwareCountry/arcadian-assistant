import React, { Component } from 'react';
import { CompanyDepartmentsLevel } from './company-departments-level';
import { connect, MapStateToProps, MapDispatchToProps, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { appendRoot, rootId } from '../reducers/people/append-root';
import { DepartmentIdToChildren, DepartmentIdToSelectedId, DepartmentNode, DepartmentIdToNode } from '../reducers/people/people.model';
import { selectCompanyDepartment } from '../reducers/people/people.action';
import { EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';

interface CompanyDepartmentsStateProps {
    departmentIdToNode: DepartmentIdToNode;
    headDepartment: DepartmentNode;
    filter: string;
    selectedCompanyDepartmentId: string;
    employeesById: EmployeeMap;
    employeeIdsByDepartment: EmployeeIdsGroupMap;
}

interface CompanyDepartmentsDispatchProps {
    selectCompanyDepartment: (departmentId: string) => void;
    loadEmployeesForDepartment: (departmentId: string) => void;
    onPressEmployee: (Employee: Employee) => void;
}

const mapStateToProps: MapStateToProps<CompanyDepartmentsStateProps, void, AppState> = (state: AppState) => ({
    departmentIdToNode: state.people.departmentIdToNodes,
    headDepartment: state.people.headDepartment,
    filter: state.people.filter,
    selectedCompanyDepartmentId: state.people.selectedCompanyDepartmentId,
    employeesById: state.organization.employees.employeesById,
    employeeIdsByDepartment: state.organization.employees.employeeIdsByDepartment
});

const mapDispatchToProps: MapDispatchToProps<CompanyDepartmentsDispatchProps, void> = (dispatch: Dispatch<any>) => ({
    selectCompanyDepartment: (departmentId: string) => {
        dispatch(selectCompanyDepartment(departmentId));
    },
    loadEmployeesForDepartment: (departmentId: string) => {
        dispatch(loadEmployeesForDepartment(departmentId));
    },
    onPressEmployee: (employee: Employee) => {
        dispatch(openEmployeeDetailsAction(employee));
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
            || !this.props.employeeIdsByDepartment.equals(nextProps.employeeIdsByDepartment);
    }

    public render() {

        const {
            employeesById,
            employeeIdsByDepartment,
            departmentIdToChildren,
            selection
        } = this.buildData();

        return this.props.headDepartment
            ? <CompanyDepartmentsLevel
                departmentId={rootId}
                departmentIdToChildren={departmentIdToChildren}
                employeesById={employeesById}
                employeeIdsByDepartment={employeeIdsByDepartment}
                selection={selection}
                onSelectedNode={this.props.selectCompanyDepartment}
                onPressEmployee={this.props.onPressEmployee}
                loadEmployeesForDepartment={this.props.loadEmployeesForDepartment} />
            : null;
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

    private filterEmployees(): EmployeeMap {
        if (!this.props.filter) {
            return this.props.employeesById;
        }
    
        return this.props.employeesById
            .filter(employee => employee.name.toLowerCase().includes(this.props.filter.toLowerCase()))
            .toMap();
    }

    private filterDepartments(employeesById: EmployeeMap): DepartmentIdToNode {
        const employees = employeesById.toArray();
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

    private buildDepartmentIdToChildren(departmentIdToNode: DepartmentIdToNode): DepartmentIdToChildren {
        const children: DepartmentIdToChildren = {};
    
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
                    || (employeesIds.size === 1 && employeesIds.has(parent.chiefId))) {
                        continue;
                }
    
                node.abbreviation = `${parent.abbreviation} Staff`;
            }
    
            children[node.parentId].push(node);
        }
    
        return children;
    }

    private buildSelectedDepartmentId(departmentIdToNode: DepartmentIdToNode, employeesById: EmployeeMap): string {
        if (departmentIdToNode.has(this.props.selectedCompanyDepartmentId)) {
            return this.props.selectedCompanyDepartmentId;
        }
    
        const firstEmployee = employeesById.first();
    
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

    private isHeadDepartmentSame(a: DepartmentNode, b: DepartmentNode): boolean {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        return a.equals(b);
    }

    private areNodesEqual(a: DepartmentIdToNode, b: DepartmentIdToNode): boolean {
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
            if (!node.equals(b.get(node.departmentId))) {
                return false;
            }
        }

        return true;
    }

    private areEmployeesEqual(a: EmployeeMap, b: EmployeeMap) {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }
        
        if (a.equals(b)) {
            return true;
        }

        const aArray = a.toArray();
        const bArray = b.toArray();

        for (let i = 0; i < aArray.length; i++) {
            if (!aArray[i].equals(bArray[i])) {
                return false;
            }
        }        

        return true;
    }       
}

export const CompanyDepartments = connect(mapStateToProps, mapDispatchToProps)(CompanyDepartmentsImpl);