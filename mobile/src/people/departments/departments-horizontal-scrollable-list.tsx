import React, { Component } from 'react';
import { Animated, View, ScrollView, Dimensions, NativeScrollEvent, NativeSyntheticEvent } from 'react-native';
import { EmployeeCardWithAvatar } from './employee-card-with-avatar';
import { Employee } from '../../reducers/organization/employee.model';
import { EmployeesStore } from '../../reducers/organization/employees.reducer';
import { Department } from '../../reducers/organization/department.model';

interface DepartmentsHScrollableListProps {
    headDepartment: Department;
    departments: Department[];
    departmentsLists: DepartmentsListStateDescriptor;
    employees: EmployeesStore;
    updateDepartmentsBranch: (departmentId: string) => void;
    requestEmployeesForDepartment: (departmentId: string) => void;
    onItemClicked: (e: Employee) => void;
}

interface ScrollViewComponent extends Component {
    scrollTo(to : {y?: number, x?: number, animated?: boolean}): void;
    scrollToEnd(): void;
}

export interface DepartmentsListStateDescriptor {
    currentPage: number;
}

export class DepartmentsHScrollableList extends Component<DepartmentsHScrollableListProps> {
    private employeeCards: EmployeeCardWithAvatar[];
    private animatedValue: Animated.Value;
    private curVisibleCard: number | null = null;

    public shouldComponentUpdate(nextProps: DepartmentsHScrollableListProps) {	
        return this.props.departmentsLists !== nextProps.departmentsLists || 
            !this.props.employees.employeesById.equals(nextProps.employees.employeesById);
    }

    public render() {
        this.employeeCards = [];
        
        const { headDepartment, departments } = this.props;
        const subDepartments = headDepartment != null && departments.length > 0 ? departments : null;

        const view = <View>
            <ScrollView 
                horizontal 
                pagingEnabled
                showsHorizontalScrollIndicator={false}
                onMomentumScrollEnd={this.onMomentumScrollEnd}
                onScrollBeginDrag={this.onScrollBeginDrag}
                ref='_scrollView'
            >
                {
                    subDepartments != null ? subDepartments.map((subDepartment, index) => 
                    <EmployeeCardWithAvatar 
                        employee={this.props.employees.employeesById.get(subDepartment.chiefId)}
                        departmentAbbreviation={subDepartment.abbreviation}
                        leftNeighbor={(index > 0) ? this.props.employees.employeesById.get(subDepartments[index - 1].chiefId) : null } 
                        rightNeighbor={(index < subDepartments.length - 1) ? this.props.employees.employeesById.get(subDepartments[index + 1].chiefId) : null} 
                        ref={(employeeCard) => { 
                            if ( employeeCard != null ) {
                                this.employeeCards.push(employeeCard);
                            }
                        }} 
                        key={subDepartment.departmentId} 
                        onItemClicked={this.props.onItemClicked}
                    />) : null
                }
            </ScrollView>
        </View>;
        this.setScroll();

        return view;
    }

    private setScroll() {
        let x;
        if (this.props.departmentsLists !== undefined) {
            this.curVisibleCard = this.props.departmentsLists.currentPage;
            x = this.props.departmentsLists.currentPage;
        } else {
            x = this.props.departments.length;
            this.curVisibleCard = null;
        }
        const curOffsetX = Dimensions.get('window').width * x;

        setTimeout(() => {
            const view = this.refs._scrollView as ScrollViewComponent;
            if (view) {
                view.scrollTo({y: 0, x: curOffsetX});
            }
        });
    }

    private onMomentumScrollEnd = (event: NativeSyntheticEvent<NativeScrollEvent>) => {
        const offset = event.nativeEvent.contentOffset;
        if (offset) {
            const currentPage = Math.round(offset.x / Dimensions.get('window').width) + 1;

            if (currentPage > this.employeeCards.length) {
                this.props.requestEmployeesForDepartment(this.props.headDepartment.departmentId);
                this.props.updateDepartmentsBranch(this.props.headDepartment.departmentId);
            } else {
                const visibleCard: EmployeeCardWithAvatar = this.employeeCards[currentPage - 1];
                visibleCard.revealNeighboursAvatars(true);
                this.curVisibleCard = currentPage - 1;

                const visibleDepartment = this.props.departments[currentPage - 1];
                this.props.updateDepartmentsBranch(visibleDepartment.departmentId);

                this.props.requestEmployeesForDepartment(visibleDepartment.departmentId);
                const childDepartment = this.props.departments.find(department => department.parentDepartmentId === visibleDepartment.departmentId);
                if (childDepartment !== undefined) {
                    this.props.requestEmployeesForDepartment(childDepartment.departmentId);
                }
            }
        }
    }

    private onScrollBeginDrag = (event: NativeSyntheticEvent<NativeScrollEvent>) => {
        if (this.curVisibleCard && this.employeeCards[this.curVisibleCard]) {
            this.employeeCards[this.curVisibleCard].revealNeighboursAvatars(false);
            this.curVisibleCard = null;
        }
    }
}