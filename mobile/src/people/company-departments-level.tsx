import React, { Component } from 'react';
import { companyDepartments, nodesContainerWidth, nodesContainerHeight } from './styles';
import { View, Animated } from 'react-native';
import { layout } from '../calendar/event-dialog/styles';
import { StyledText } from '../override/styled-text';
import { DepartmentIdToChildren, MapDepartmentNode, EmployeeIdToNode, DepartmentIdToSelectedId, MapEmployeeNode } from '../reducers/people/people.model';
import { Set, Map } from 'immutable';
import { CompanyDepartmentsLevelNodes } from './company-departments-level-nodes';
import { EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { CompanyDepartmentsLevelPeople } from './company-departments-level-people';

interface CompanyDepartmentsLevelProps {
    departmentId: string;
    departmentIdToChildren: DepartmentIdToChildren;
    employeeIdToNode: EmployeeIdToNode;
    selection: DepartmentIdToSelectedId;
    onSelectedNode: (departmentId: string) => void;
    onPressEmployee: (employeeId: string) => void;
}

export class CompanyDepartmentsLevel extends Component<CompanyDepartmentsLevelProps> {
    public render() {
        const { departmentIdToChildren, departmentId } = this.props;
        const nodes = departmentIdToChildren[departmentId];

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

    private renderNodes(nodes: Set<MapDepartmentNode>) {
        const selectedDepartmentId = this.props.selection[this.props.departmentId];
        const chiefs = this.getChiefs(nodes);

        return (
            <CompanyDepartmentsLevelNodes
                width={nodesContainerWidth} 
                height={nodesContainerHeight}
                nodes={nodes} 
                chiefs={chiefs} 
                selectedDepartmentId={selectedDepartmentId}
                onNextDepartment={this.props.onSelectedNode}
                onPrevDepartment={this.props.onSelectedNode}
                onPressChief={this.props.onPressEmployee} />
        );
    }

    private renderSubLevel(nodes: Set<MapDepartmentNode>): JSX.Element {
        const selectedDepartmentId = this.props.selection[this.props.departmentId];
        const selectedDepartmentNode = nodes.find(node => node.get('departmentId') === selectedDepartmentId);

        if (selectedDepartmentNode) {
            return this.renderDepartmentsLevel(selectedDepartmentNode);
        }

        const first = nodes.first();

        return this.renderDepartmentsLevel(first);
    }

    private renderDepartmentsLevel(node: MapDepartmentNode) {
        return (
            <CompanyDepartmentsLevel
                departmentId={node.get('departmentId')}
                departmentIdToChildren={this.props.departmentIdToChildren}
                employeeIdToNode={this.props.employeeIdToNode}
                selection={this.props.selection}
                onSelectedNode={this.props.onSelectedNode}
                onPressEmployee={this.props.onPressEmployee} />
        );
    }

    private renderDepartmentPeople() {
        const people = this.props.employeeIdToNode.filter(employeeNode => employeeNode.get('departmentId') === this.props.departmentId).toMap();

        return (
            <CompanyDepartmentsLevelPeople employeeIdToNode={people} onPressEmployee={this.props.onPressEmployee} />
        );
    }

    private getChiefs(nodes: Set<MapDepartmentNode>): EmployeeIdToNode {
        const chiefIdToChiefs = nodes.map(node => {
            const chiefId = node.get('chiefId');
            return [chiefId, this.props.employeeIdToNode.get(chiefId)];
        });
        return Map<string, MapEmployeeNode>(chiefIdToChiefs);
    }    
}