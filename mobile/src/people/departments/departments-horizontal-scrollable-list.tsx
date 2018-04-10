import React, { Component, SyntheticEvent } from 'react';
import { Animated, Easing, View, Text, ScrollView, Dimensions, TouchableOpacity, NativeScrollEvent, NativeSyntheticEvent } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { EmployeeCardWithAvatar } from '../employee-card-with-avatar';
import { DepartmentsTreeNode, stubIdForSubordinates } from './departments-tree-node';
import { Employee } from '../../reducers/organization/employee.model';
import { PeopleActions, updateDepartmentIdsTree } from '../../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../../reducers/organization/organization.action';
import { EmployeesStore } from '../../reducers/organization/employees.reducer';

interface DepartmentsHScrollableListProps {
    treeLevel?: number;
    departmentsTreeNodes?: DepartmentsTreeNode[];
    headDepartment: DepartmentsTreeNode;
    employees?: EmployeesStore;
    topOffset?: number;
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentIdsTree: (index: number, department: DepartmentsTreeNode) => void;
    onItemClicked: (e: Employee) => void;
    employeesPredicate: (employee: Employee) => boolean;
}

export class DepartmentsHScrollableList extends Component<DepartmentsHScrollableListProps> {
    private employeeCards: EmployeeCardWithAvatar[];
    private animatedValue: Animated.Value;
    private buttonText: Text;
    private currentPage: number = null;
    private forceComponentRender = false;

    public componentDidMount() {
        if (this.props.departmentsTreeNodes !== null) {
            this.props.requestEmployeesForDepartment(this.props.departmentsTreeNodes[0].departmentId);
        }
    }

    public shouldComponentUpdate(nextProps: DepartmentsHScrollableListProps) {
        if (this.forceComponentRender) {
            this.forceComponentRender = false;
            return true;
        } else if (this.employeeCards.length > 0 && this.currentPage !== null && this.currentPage <= this.employeeCards.length) {
            const visibleCard: EmployeeCardWithAvatar = this.employeeCards[this.currentPage - 1];
            const currentEmployeeRef = this.props.employees.employeesById.get(visibleCard.props.employee.employeeId);
            const nextEmployeeRef = nextProps.employees.employeesById.get(visibleCard.props.employee.employeeId);

            if (currentEmployeeRef !== nextEmployeeRef) {
                return true;
            } else {
                return false;
            }
        } else {
            const employees = this.props.employees.employeesById.filter(this.props.employeesPredicate);
            const nextEmployees = nextProps.employees.employeesById.filter(this.props.employeesPredicate);

            if (!employees.equals(nextEmployees)) {
                return true;
            } else {
                return false;
            }
        }
    }

    public render() {
        this.employeeCards = [];
        
        const { headDepartment } = this.props;
        const subDepartments = headDepartment != null && headDepartment.children != null ? headDepartment.children : null;
        let subordinates;
        
        if (this.props.employees.employeeIdsByDepartment.has(headDepartment.departmentId) && this.props.employees.employeeIdsByDepartment.get(headDepartment.departmentId).size > 0) {
            subordinates = this.props.employees.employeeIdsByDepartment.get(headDepartment.departmentId)
                            .filter((employeeId) => employeeId !== headDepartment.departmentChiefId)
                            .map(x => this.props.employees.employeesById.get(x)).toArray();
        } else {
            subordinates = null;
        }

        return <View>
            <Animated.ScrollView 
                horizontal 
                pagingEnabled 
                showsHorizontalScrollIndicator={false}
                onMomentumScrollEnd={this.onMomentumScrollEnd}
                onScrollBeginDrag={this.onScrollBeginDrag}
            >
                {
                    subDepartments != null ? subDepartments.map(subDepartment => <EmployeeCardWithAvatar 
                        employee={this.props.employees.employeesById.get(subDepartment.departmentChiefId)}
                        departmentAbbreviation={subDepartment.departmentAbbreviation} 
                        treeLevel={this.props.treeLevel}
                        leftNeighbor={(subDepartments.indexOf(subDepartment) > 0 && subDepartments.length > 1) ? this.props.employees.employeesById.get(subDepartments[subDepartments.indexOf(subDepartment) - 1].departmentChiefId) : null } 
                        rightNeighbor={(subDepartments.indexOf(subDepartment) < subDepartments.length - 1 && subDepartments.length > 1) ? this.props.employees.employeesById.get(subDepartments[subDepartments.indexOf(subDepartment) + 1].departmentChiefId) : null} 
                        ref={(employeeCard) => { 
                            if ( employeeCard != null ) {
                                this.employeeCards.push(employeeCard);
                            }
                        }} 
                        key={subDepartment.departmentId} 
                        onItemClicked={this.props.onItemClicked}
                    />) : null
                }
                
                {
                    (subordinates != null && subordinates.length > 0) ? 
                        <EmployeeCardWithAvatar 
                            employees={subordinates} 
                            chiefId={headDepartment.departmentChiefId}
                            treeLevel={this.props.treeLevel} 
                            stretchToFitScreen={subDepartments === null || this.currentPage > subDepartments.length}
                            onItemClicked={this.props.onItemClicked}
                        /> 
                            : null
                }
            </Animated.ScrollView>
        </View>;
    }

    private onMomentumScrollEnd = (event: NativeSyntheticEvent<NativeScrollEvent>) => {
        const offset = event.nativeEvent.contentOffset;
        if (offset) {
            this.currentPage = Math.round(offset.x / Dimensions.get('window').width) + 1;
            this.forceComponentRender = true;

            if (this.currentPage > this.employeeCards.length) {
                this.props.requestEmployeesForDepartment(this.props.headDepartment.departmentId);
                const stubDN: DepartmentsTreeNode = {
                    departmentAbbreviation: null,
                    departmentChiefId: this.props.headDepartment.departmentChiefId,
                    parent: null,
                    children: null,
                    departmentId: stubIdForSubordinates
                };
                this.props.updateDepartmentIdsTree(this.props.treeLevel, stubDN);
            } else {
                const visibleCard: EmployeeCardWithAvatar = this.employeeCards[this.currentPage - 1];
                visibleCard.revealNeighboursAvatars(true);
                const visibleDepartment = this.props.headDepartment.children[this.currentPage - 1];
                this.props.updateDepartmentIdsTree(this.props.treeLevel, visibleDepartment);
                this.props.requestEmployeesForDepartment(visibleDepartment.departmentId);
                if (visibleDepartment.children !== null && visibleDepartment.children.length > 0) {
                    this.props.requestEmployeesForDepartment(visibleDepartment.children[0].departmentId);
                }
            }
        }
    }

    private onScrollBeginDrag = (event: NativeSyntheticEvent<NativeScrollEvent>) => {
        this.employeeCards.forEach(card => {
            card.revealNeighboursAvatars(false);
        });
    }
}