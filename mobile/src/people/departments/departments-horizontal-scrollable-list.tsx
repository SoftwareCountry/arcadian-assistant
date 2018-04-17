import React, { Component, SyntheticEvent } from 'react';
import { Animated, Easing, View, Text, ScrollView, Dimensions, TouchableOpacity, NativeScrollEvent, NativeSyntheticEvent, ScrollViewStatic } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { EmployeeCardWithAvatar } from '../employee-card-with-avatar';
import { Employee } from '../../reducers/organization/employee.model';
import { EmployeesStore } from '../../reducers/organization/employees.reducer';
import { Department } from '../../reducers/organization/department.model';

interface DepartmentsHScrollableListProps {
    treeLevel?: number;
    headDepartmentId?: string;
    headDepartmentChiefId?: string;
    departments?: Department[];
    departmentsLists?: DepartmentsListStateDescriptor;
    focusOnDepartmentWithId?: string;
    currentFocusedDepartmentId?: string;
    employees?: EmployeesStore;
    topOffset?: number;
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentsBranch: (departmentId: string) => void;
    onItemClicked: (e: Employee) => void;
    employeesPredicate: (employee: Employee) => boolean;
}

interface AAScrollViewComponent extends Component {
    scrollTo(y?: number | { x?: number; y?: number; animated?: boolean }, x?: number, animated?: boolean): void;
    scrollToEnd(): void;
}

export interface DepartmentsListStateDescriptor {
    currentPage: number;
    // forceComponentRender: boolean;
    // manualScrollMode: boolean;
}

export class DepartmentsHScrollableList extends Component<DepartmentsHScrollableListProps> {
    private scrollView: AAScrollViewComponent;
    private employeeCards: EmployeeCardWithAvatar[];
    private animatedValue: Animated.Value;
    private currentPage: number = null;
    private forceComponentRender = false;
    private manualScrollMode = true;

    public shouldComponentUpdate(nextProps: DepartmentsHScrollableListProps) {
        if (this.forceComponentRender) {
            this.forceComponentRender = false;
            return true;
        } else if (this.props.departmentsLists !== nextProps.departmentsLists) {
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
        
        const { headDepartmentId, headDepartmentChiefId, departments } = this.props;
        const subDepartments = headDepartmentId != null && departments.length > 0 ? departments : null;
        let subordinates;
        
        if (this.props.employees.employeeIdsByDepartment.has(headDepartmentId) && this.props.employees.employeeIdsByDepartment.get(headDepartmentId).size > 0) {
            subordinates = this.props.employees.employeesById.filter(this.props.employeesPredicate).toArray();
        } else {
            subordinates = null;
        }

        return <View>
            <ScrollView 
                horizontal 
                pagingEnabled
                contentOffset={{x: Dimensions.get('window').width * (this.props.departmentsLists !== undefined ? this.props.departmentsLists.currentPage : 0), y: 0}}
                showsHorizontalScrollIndicator={false}
                onMomentumScrollEnd={this.onMomentumScrollEnd}
                onScrollBeginDrag={this.onScrollBeginDrag}
                ref={ref => this.scrollView = ref as AAScrollViewComponent}
            >
                {
                    subDepartments != null ? subDepartments.map((subDepartment, index) => <EmployeeCardWithAvatar 
                        employee={this.props.employees.employeesById.get(subDepartment.chiefId)}
                        departmentAbbreviation={subDepartment.abbreviation} 
                        treeLevel={this.props.treeLevel}
                        leftNeighbor={(index > 0 && subDepartments.length > 1) ? this.props.employees.employeesById.get(subDepartments[index - 1].chiefId) : null } 
                        rightNeighbor={(index < subDepartments.length - 1 && subDepartments.length > 1) ? this.props.employees.employeesById.get(subDepartments[index + 1].chiefId) : null} 
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
                            employees={subordinates.sort()} 
                            chiefId={headDepartmentChiefId}
                            treeLevel={this.props.treeLevel} 
                            stretchToFitScreen={subDepartments === null || this.currentPage > subDepartments.length}
                            onItemClicked={this.props.onItemClicked}
                        /> 
                            : null
                }
            </ScrollView>
        </View>;
    }

    private onMomentumScrollEnd = (event: NativeSyntheticEvent<NativeScrollEvent>) => {
        const offset = event.nativeEvent.contentOffset;
        if (offset) {
            this.currentPage = Math.round(offset.x / Dimensions.get('window').width) + 1;
            this.forceComponentRender = true;

            if (this.currentPage > this.employeeCards.length) {
                this.props.requestEmployeesForDepartment(this.props.headDepartmentId);
                if (this.manualScrollMode) {
                    this.props.updateDepartmentsBranch(this.props.headDepartmentId);
                } else {
                    this.manualScrollMode = true;
                }
            } else {
                const visibleCard: EmployeeCardWithAvatar = this.employeeCards[this.currentPage - 1];
                visibleCard.revealNeighboursAvatars(true);
                const visibleDepartment = this.props.departments[this.currentPage - 1];
                if (this.manualScrollMode) {
                    this.props.updateDepartmentsBranch(visibleDepartment.departmentId);
                } else {
                    this.manualScrollMode = true;
                }
                this.props.requestEmployeesForDepartment(visibleDepartment.departmentId);
                const childDepartment = this.props.departments.find(department => department.parentDepartmentId === visibleDepartment.departmentId);
                if (childDepartment !== undefined) {
                    this.props.requestEmployeesForDepartment(childDepartment.departmentId);
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