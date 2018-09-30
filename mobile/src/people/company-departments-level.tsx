import React, { Component } from 'react';
import { companyDepartments } from './styles';
import { View } from 'react-native';
import { layout } from '../calendar/event-dialog/styles';
import { StyledText } from '../override/styled-text';
import { DepartmentIdToChildren, MapDepartmentNode } from '../reducers/people/people.model';
import { Set } from 'immutable';
import { CompanyDepartmentsLevelNodes } from './company-departments-level-node';
import { EmployeeIdsGroupMap, EmployeeMap } from '../reducers/organization/employees.reducer';

interface CompanyDepartmentsLevelProps {
    departmentId: string;
    departmentIdToChildren: DepartmentIdToChildren;
    employeeIdsByDepartment: EmployeeIdsGroupMap;
    employeesById: EmployeeMap;
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
        return <CompanyDepartmentsLevelNodes nodes={nodes} employeesById={this.props.employeesById} />;
    }

    private renderSubLevel(): JSX.Element {
        const nodes = this.props.departmentIdToChildren[this.props.departmentId];

        if (!nodes) {
            return null;
        }

        const first = nodes.first();

        if (!first) {
            return null;
        }

        return <CompanyDepartmentsLevel
            departmentId={first.get('departmentId')}
            departmentIdToChildren={this.props.departmentIdToChildren}
            employeeIdsByDepartment={this.props.employeeIdsByDepartment}
            employeesById={this.props.employeesById} />;
    }
}