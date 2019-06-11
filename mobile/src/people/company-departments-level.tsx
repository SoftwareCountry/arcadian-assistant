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

//============================================================================
interface CompanyDepartmentsLevelProps {
    departmentId: string;
    departmentNode?: DepartmentNode;
    staffDepartmentId?: string;
    departmentIdToChildren: DepartmentIdToChildren;
    employeesById: Nullable<EmployeeMap>;
    employeeIdsByDepartment: Nullable<EmployeeIdsGroupMap>;
    selection: DepartmentIdToSelectedId;
    onSelectedNode: (departmentId: string) => void;
    onPressEmployee: (employee: Employee) => void;
}

//============================================================================
export class CompanyDepartmentsLevel extends Component<CompanyDepartmentsLevelProps> {

    //----------------------------------------------------------------------------
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

    //----------------------------------------------------------------------------
    private renderNodes(nodes: DepartmentNode[]) {
        const { selection, departmentId, onSelectedNode, onPressEmployee, employeesById } = this.props;

        const selectedDepartmentId = selection[departmentId];
        const chiefs = nodes.filter(node => !!node.chiefId)
            .map(node => {
                if (!employeesById) {
                    return null;
                }
                return employeesById.get(node.chiefId!);
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
                onNextDepartment={onSelectedNode}
                onPrevDepartment={onSelectedNode}
                onPressChief={onPressEmployee}/>
        );
    }

    //----------------------------------------------------------------------------
    private renderSubLevel(nodes: DepartmentNode[]): JSX.Element {
        const { selection, departmentId } = this.props;

        const selectedDepartmentId = selection[departmentId];
        const selectedDepartmentNode = nodes.find(node => node.departmentId === selectedDepartmentId);

        if (selectedDepartmentNode) {
            return this.renderDepartmentsLevel(selectedDepartmentNode);
        }

        const first = nodes[0];

        return this.renderDepartmentsLevel(first);
    }

    //----------------------------------------------------------------------------
    private renderDepartmentsLevel(node: DepartmentNode) {
        const {
            selection, departmentIdToChildren, employeeIdsByDepartment, employeesById, onPressEmployee, onSelectedNode
        } = this.props;

        return (
            <CompanyDepartmentsLevel
                departmentId={node.departmentId}
                departmentNode={node}
                staffDepartmentId={node.staffDepartmentId ? node.staffDepartmentId : undefined}
                departmentIdToChildren={departmentIdToChildren}
                employeesById={employeesById}
                employeeIdsByDepartment={employeeIdsByDepartment}
                selection={selection}
                onSelectedNode={onSelectedNode}
                onPressEmployee={onPressEmployee}/>
        );
    }

    //----------------------------------------------------------------------------
    private renderDepartmentPeople() {
        const {
            staffDepartmentId, departmentId, employeeIdsByDepartment, employeesById, onPressEmployee, departmentNode
        } = this.props;

        if (!employeeIdsByDepartment || !departmentNode) {
            return null;
        }

        const departmentToRenderId = staffDepartmentId ? staffDepartmentId : departmentId;

        const employeeIds = employeeIdsByDepartment.get(departmentToRenderId);

        if (!employeeIds) {
            return null;
        }

        const employees = employeeIds
            .map(employeeId => {
                if (!employeesById) {
                    return null;
                }
                return employeesById.get(employeeId);
            })
            .filter(employee => !!employee)
            .map(employee => employee!)
            .toArray();

        const chief = employees.find(employee => employee.employeeId === departmentNode.chiefId);

        return (
            <CompanyDepartmentsLevelPeople employees={employees} onPressEmployee={onPressEmployee} chief={chief}/>
        );
    }
}
