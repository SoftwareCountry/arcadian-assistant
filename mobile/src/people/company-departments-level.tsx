import React, { Component } from 'react';
import { companyDepartments, nodesContainerHeight, nodesContainerWidth } from './styles';
import { View } from 'react-native';
import { DepartmentIdToChildren, DepartmentIdToSelectedId, DepartmentNode } from '../reducers/people/people.model';
import { CompanyDepartmentsLevelNodes } from './company-departments-level-nodes';
import { EmployeeIdsGroupMap, EmployeeMap } from '../reducers/organization/employees.reducer';
import { CompanyDepartmentsLevelPeople } from './company-departments-level-people';
import { departmentNodeComparer } from './department-comparer';
import { Employee } from '../reducers/organization/employee.model';
import { Nullable } from 'types';

interface CompanyDepartmentsLevelProps {
    departmentId: string;
    staffDepartmentId?: string;
    departmentIdToChildren: DepartmentIdToChildren;
    employeesById: Nullable<EmployeeMap>;
    employeeIdsByDepartment: Nullable<EmployeeIdsGroupMap>;
    selection: DepartmentIdToSelectedId;
    onSelectedNode: (departmentId: string) => void;
    onPressEmployee: (employee: Employee) => void;
    loadEmployeesForDepartment: (departmentId: string) => void;
}

export class CompanyDepartmentsLevel extends Component<CompanyDepartmentsLevelProps> {
    public render() {
        const { departmentIdToChildren, departmentId } = this.props;

        const children = departmentIdToChildren[departmentId];
        const nodes = children && children.length !== 0
            ? children.sort((a, b) => departmentNodeComparer(a, b))
            : null;

        return (
            <View style={companyDepartments.levelContainer}>
                {
                    nodes && this.renderNodes(nodes)
                }
                {
                    nodes ? this.renderSubLevel(nodes) : this.renderDepartmentPeople()
                }
            </View>
        );
    }

    private renderNodes(nodes: DepartmentNode[]) {
        const selectedDepartmentId = this.props.selection[this.props.departmentId];
        const chiefs = nodes.filter(node => !!node.chiefId)
            .map(node => {
                if (!this.props.employeesById) {
                    return null;
                }
                return this.props.employeesById.get(node.chiefId!)
            })
            .filter(chief => !!chief)
            .map(chief => chief!);

        return (
            <CompanyDepartmentsLevelNodes
                width={nodesContainerWidth}
                height={nodesContainerHeight}
                nodes={nodes}
                chiefs={chiefs}
                selectedDepartmentId={selectedDepartmentId}
                onNextDepartment={this.props.onSelectedNode}
                onPrevDepartment={this.props.onSelectedNode}
                onPressChief={this.props.onPressEmployee}
                loadEmployeesForDepartment={this.props.loadEmployeesForDepartment}/>
        );
    }

    private renderSubLevel(nodes: DepartmentNode[]): JSX.Element {
        const selectedDepartmentId = this.props.selection[this.props.departmentId];
        const selectedDepartmentNode = nodes.find(node => node.departmentId === selectedDepartmentId);

        if (selectedDepartmentNode) {
            return this.renderDepartmentsLevel(selectedDepartmentNode);
        }

        const first = nodes[0];

        return this.renderDepartmentsLevel(first);
    }

    private renderDepartmentsLevel(node: DepartmentNode) {
        return (
            <CompanyDepartmentsLevel
                departmentId={node.departmentId}
                staffDepartmentId={node.staffDepartmentId ? node.staffDepartmentId : undefined}
                departmentIdToChildren={this.props.departmentIdToChildren}
                employeesById={this.props.employeesById}
                employeeIdsByDepartment={this.props.employeeIdsByDepartment}
                selection={this.props.selection}
                onSelectedNode={this.props.onSelectedNode}
                onPressEmployee={this.props.onPressEmployee}
                loadEmployeesForDepartment={this.props.loadEmployeesForDepartment}/>
        );
    }

    private renderDepartmentPeople() {
        const departmentId = this.props.staffDepartmentId
            ? this.props.staffDepartmentId
            : this.props.departmentId;

        if (!this.props.employeeIdsByDepartment) {
            return null;
        }

        const employeeIds = this.props.employeeIdsByDepartment.get(departmentId);

        if (!employeeIds) {
            return null;
        }

        const employees = employeeIds
            .map(employeeId => {
                if (!this.props.employeesById) {
                    return null;
                }
                return this.props.employeesById.get(employeeId);
            })
            .filter(employee => !!employee)
            .map(employee => employee!)
            .toArray();

        return (
            <CompanyDepartmentsLevelPeople employees={employees} onPressEmployee={this.props.onPressEmployee}/>
        );
    }
}
