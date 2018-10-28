import React, { Component } from 'react';
import { CompanyDepartmentsLevel } from './company-departments-level';
import { connect, MapStateToProps, MapDispatchToProps, Dispatch } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { appendRoot, rootId } from '../reducers/people/append-root';
import { filterDepartments } from '../reducers/people/filter-departments';
import { buildBranchFromChildToParent } from '../reducers/people/build-branch-from-child-to-parent';
import { buildDepartmentIdToChildren } from '../reducers/people/build-department-children';
import { Department } from '../reducers/organization/department.model';
import { DepartmentIdToChildren, DepartmentIdToSelectedId, DepartmentNode, DepartmentIdToNode } from '../reducers/people/people.model';
import { filterEmployees } from '../reducers/people/filter-employees';
import { buildDepartmentsSelection } from '../reducers/people/build-departments-selection';
import { selectCompanyDepartment } from '../reducers/people/people.action';
import { buildSelectedDepartmentId } from '../reducers/people/build-selected-department-id';
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
        return !this.isNodesEqual(this.props.departmentIdToNode, nextProps.departmentIdToNode)
            || !this.isHeadDepartmentEqual(this.props.headDepartment, nextProps.headDepartment)
            || this.props.filter !== nextProps.filter
            || this.props.selectedCompanyDepartmentId !== nextProps.selectedCompanyDepartmentId
            || !this.isEmployeesEqual(this.props.employeesById, nextProps.employeesById)
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
  
        const employeesById = filterEmployees(
            this.props.employeesById, 
            this.props.filter);
    
        let mapDepartmentIdToNode = this.props.departmentIdToNode;
        
        if (this.props.filter) {
            const filteredDepartmentsNodes = filterDepartments(this.props.departmentIdToNode, employeesById);
            mapDepartmentIdToNode = buildBranchFromChildToParent(filteredDepartmentsNodes, this.props.departmentIdToNode);
        }
    
        const departmentIdToChildren = buildDepartmentIdToChildren(mapDepartmentIdToNode, this.props.employeeIdsByDepartment);
    
        const selectedCompanyDepartmentId = buildSelectedDepartmentId(mapDepartmentIdToNode, employeesById, this.props.selectedCompanyDepartmentId);
    
        const selection = selectedCompanyDepartmentId 
            ? buildDepartmentsSelection(this.props.departmentIdToNode, selectedCompanyDepartmentId)
            : {};
    
        return {
            employeesById: employeesById,
            employeeIdsByDepartment: this.props.employeeIdsByDepartment,
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

    private isNodesEqual(a: DepartmentIdToNode, b: DepartmentIdToNode): boolean {
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

        for (let i = 0; i < aArray.length; i++) {
            if (!aArray[i].equals(bArray[i])) {
                return false;
            }
        }        

        return true;
    }       
}

export const CompanyDepartments = connect(mapStateToProps, mapDispatchToProps)(CompanyDepartmentsImpl);