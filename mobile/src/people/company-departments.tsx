import React, { Component } from 'react';
import { CompanyDepartmentsLevel } from './company-departments-level';
import { connect, MapStateToProps, MapDispatchToProps, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { buildDepartmentIdToNode } from '../reducers/people/build-department-id-to-node';
import { appendRoot, rootId } from '../reducers/people/append-root';
import { filterDepartments } from '../reducers/people/filter-departments';
import { buildBranchFromChildToParent } from '../reducers/people/build-branch-from-child-to-parent';
import { buildDepartmentIdToChildren } from '../reducers/people/build-department-children';
import { Department } from '../reducers/organization/department.model';
import { DepartmentIdToChildren, DepartmentIdToSelectedId, DepartmentNode } from '../reducers/people/people.model';
import { buildEmployeeNodes } from '../reducers/people/build-employee-nodes';
import { buildDepartmentsSelection } from '../reducers/people/build-departments-selection';
import { selectCompanyDepartment } from '../reducers/people/people.action';
import { buildSelectedDepartmentId } from '../reducers/people/build-selected-department-id';
import { EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { loadEmployeesForDepartment } from '../reducers/organization/organization.action';
import { Employee } from '../reducers/organization/employee.model';
import { openEmployeeDetailsAction } from '../employee-details/employee-details-dispatcher';

interface CompanyDepartmentsStateProps {
    departmentNodes: DepartmentNode[];
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
    departmentNodes: state.people.departmentNodes,
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
        return !this.isNodesEqual(this.props.departmentNodes, nextProps.departmentNodes)
            || !this.isHeadDepartmentEqual(this.props.headDepartment, nextProps.headDepartment)
            || this.props.filter !== nextProps.filter
            || this.props.selectedCompanyDepartmentId !== nextProps.selectedCompanyDepartmentId
            || !this.isEmployeesEqual(this.props.employeesById, nextProps.employeesById)
            || !this.props.employeeIdsByDepartment.equals(nextProps.employeeIdsByDepartment);
    }

    public render() {

        const {
            employeesById,
            departmentIdToChildren,
            selection
        } = this.buildData();

        return this.props.headDepartment
            ? <CompanyDepartmentsLevel
                departmentId={rootId}
                departmentIdToChildren={departmentIdToChildren}
                employeesById={employeesById}
                selection={selection}
                onSelectedNode={this.props.selectCompanyDepartment}
                onPressEmployee={this.props.onPressEmployee}
                loadEmployeesForDepartment={this.props.loadEmployeesForDepartment} />
            : null;
    }

    private buildData() {
        const departmentIdToNode = buildDepartmentIdToNode(this.props.departmentNodes);

        appendRoot(this.props.headDepartment, departmentIdToNode);
    
        const employeesById = buildEmployeeNodes(
            this.props.employeesById, 
            this.props.filter);
    
        let mapDepartmentIdToNode = departmentIdToNode;
        
        if (this.props.filter) {
            const filteredDepartmentsNodes = filterDepartments(this.props.departmentNodes, employeesById);
            mapDepartmentIdToNode = buildBranchFromChildToParent(filteredDepartmentsNodes, departmentIdToNode);
        }
    
        const departmentIdToChildren = buildDepartmentIdToChildren(mapDepartmentIdToNode, this.props.employeeIdsByDepartment);
    
        const selectedCompanyDepartmentId = buildSelectedDepartmentId(mapDepartmentIdToNode, employeesById, this.props.selectedCompanyDepartmentId);
    
        const selection = selectedCompanyDepartmentId 
            ? buildDepartmentsSelection(departmentIdToNode, selectedCompanyDepartmentId)
            : {};
    
        return {
            employeesById: employeesById,
            departmentIdToChildren: departmentIdToChildren,
            selection: selection
        };        
    }

    private isHeadDepartmentEqual(a: DepartmentNode, b: DepartmentNode): boolean {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        return a.equals(b);
    }

    private isNodesEqual(a: DepartmentNode[], b: DepartmentNode[]): boolean {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        if (a.length !== b.length) {
            return false;
        }

        for (let i = 0; i < a.length; i++) {
            if (!a[i].equals(b[i])) {
                return false;
            }
        }

        return true;
    }

    private isEmployeesEqual(a: EmployeeMap, b: EmployeeMap) {
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

        if (aArray.length !== bArray.length) {
            return false;
        }

        for (let i = 0; i < aArray.length; i++) {
            if (!aArray[i].equals(bArray[i])) {
                return false;
            }
        }        

        return true;
    }       
}

export const CompanyDepartments = connect(mapStateToProps, mapDispatchToProps)(CompanyDepartmentsImpl);