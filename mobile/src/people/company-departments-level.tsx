import React, { Component } from 'react';
import { companyDepartments } from './styles';
import { View } from 'react-native';
import { layout } from '../calendar/event-dialog/styles';
import { StyledText } from '../override/styled-text';
import { DepartmentIdToChildren, MapDepartmentNode, EmployeeIdToNode, DepartmentIdToSelectedId } from '../reducers/people/people.model';
import { Set } from 'immutable';
import { CompanyDepartmentsLevelNodes } from './company-departments-level-node';
import { EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';

interface CompanyDepartmentsLevelProps {
    departmentId: string;
    departmentIdToChildren: DepartmentIdToChildren;
    employeeIdsByDepartment: EmployeeIdsGroupMap;
    employeeIdToNode: EmployeeIdToNode;
    selection: DepartmentIdToSelectedId;
    onSelectedNode: (departmentId: string, allowSelect: boolean) => void;
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
                    this.renderSubLevel()
                }
            </View>
        );
    }

    private renderNodes(nodes: Set<MapDepartmentNode>) {
        const selection = this.props.selection[this.props.departmentId];
        return (
            <CompanyDepartmentsLevelNodes 
                nodes={nodes} 
                employeeIdToNode={this.props.employeeIdToNode} 
                selectedDepartmentId={selection ? selection.selectedDepartmentId : null}
                allowSelect={selection ? selection.allowSelect : false}
                onNextDepartment={this.onSelectedNode}
                onPrevDepartment={this.onSelectedNode} />
        );
    }

    private renderSubLevel(): JSX.Element {
        const nodes = this.props.departmentIdToChildren[this.props.departmentId];

        if (!nodes) {
            return null;
        }

        const selection = this.props.selection[this.props.departmentId];
        const selectedDepartmentNode = nodes.find(node => selection && node.get('departmentId') ===  selection.selectedDepartmentId);

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
                employeeIdsByDepartment={this.props.employeeIdsByDepartment}
                employeeIdToNode={this.props.employeeIdToNode}
                selection={this.props.selection}
                onSelectedNode={this.props.onSelectedNode} />
        );
    }

    private onSelectedNode = (departmentId: string) => {
        this.props.onSelectedNode(departmentId, false);
    }
}