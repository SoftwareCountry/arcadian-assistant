import React, { Component } from 'react';
import { companyDepartments } from './styles';
import { View } from 'react-native';
import { layout } from '../calendar/event-dialog/styles';
import { StyledText } from '../override/styled-text';
import { DepartmentIdToChildren, MapDepartmentNode, EmployeeIdToNode, DepartmentIdToSelectedId } from '../reducers/people/people.model';
import { Set } from 'immutable';
import { CompanyDepartmentsLevelNodes } from './company-departments-level-node';
import { EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';
import { CompanyDepartmentsLevelNodesContainer } from './company-departments-level-nodes-container';

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
        const selectedDepartmentId = this.props.selection[this.props.departmentId];

        return (
            <CompanyDepartmentsLevelNodesContainer>
                {
                    (width: number, height: number) => 
                        <CompanyDepartmentsLevelNodes
                            width={width} 
                            height={height}
                            nodes={nodes} 
                            employeeIdToNode={this.props.employeeIdToNode} 
                            selectedDepartmentId={selectedDepartmentId}
                            onNextDepartment={this.onSelectedNode}
                            onPrevDepartment={this.onSelectedNode} />
                }
            </CompanyDepartmentsLevelNodesContainer>
        );
    }

    private renderSubLevel(): JSX.Element {
        const nodes = this.props.departmentIdToChildren[this.props.departmentId];

        if (!nodes) {
            return null;
        }

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