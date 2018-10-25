import React, { Component } from 'react';
import { companyDepartments, nodesContainerWidth, nodesContainerHeight } from './styles';
import { View, Animated } from 'react-native';
import { layout } from '../calendar/event-dialog/styles';
import { StyledText } from '../override/styled-text';
import { DepartmentIdToChildren, DepartmentIdToSelectedId, DepartmentNode } from '../reducers/people/people.model';
import { Set, Map } from 'immutable';
import { CompanyDepartmentsLevelNodes } from './company-departments-level-nodes';
import { EmployeeIdsGroupMap, EmployeeMap } from '../reducers/organization/employees.reducer';
import { CompanyDepartmentsLevelPeople } from './company-departments-level-people';
import { departmentAZComparer } from './department-comparer';
import { Employee } from '../reducers/organization/employee.model';

interface CompanyDepartmentsLevelProps {
    departmentId: string;
    staffDepartmentId?: string;
    departmentIdToChildren: DepartmentIdToChildren;
    employeesById: EmployeeMap;
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
            ? children.sort((a, b) => departmentAZComparer(a, b))
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
        const chiefs = nodes.map(node => this.props.employeesById.get(node.chiefId)).filter(x => !!x);

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
                loadEmployeesForDepartment={this.props.loadEmployeesForDepartment} />
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
                staffDepartmentId={node.staffDepartmentId}
                departmentIdToChildren={this.props.departmentIdToChildren}
                employeesById={this.props.employeesById}
                selection={this.props.selection}
                onSelectedNode={this.props.onSelectedNode}
                onPressEmployee={this.props.onPressEmployee}
                loadEmployeesForDepartment={this.props.loadEmployeesForDepartment} />
        );
    }

    private renderDepartmentPeople() {
        const departmentId = this.props.staffDepartmentId 
            ? this.props.staffDepartmentId 
            : this.props.departmentId;

        const people = this.props.employeesById.filter(employeeNode => employeeNode.departmentId === departmentId).toMap();

        return (
            <CompanyDepartmentsLevelPeople employeesById={people} onPressEmployee={this.props.onPressEmployee} />
        );
    }   
}