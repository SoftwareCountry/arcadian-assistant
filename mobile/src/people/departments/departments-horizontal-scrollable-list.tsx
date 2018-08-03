import React, { Component, Ref } from 'react';
import { View, ScrollView, ScrollViewProps, Dimensions, NativeScrollEvent, NativeSyntheticEvent, InteractionManager } from 'react-native';
import { EmployeeCardWithAvatar } from './employee-card-with-avatar';
import { Employee } from '../../reducers/organization/employee.model';
import { EmployeesStore } from '../../reducers/organization/employees.reducer';
import { Department } from '../../reducers/organization/department.model';
import { DepartmentsListStateDescriptor } from '../../reducers/people/people.reducer';

interface DepartmentsHScrollableListProps {
    headDepartment: Department;
    departments: Department[];
    departmentsLists: DepartmentsListStateDescriptor;
    employees: EmployeesStore;
    updateDepartmentsBranch: (departmentId: string) => void;
    onItemClicked: (e: Employee) => void;
    key: string;
}

function arrayEquals(a: Department[], b: Department[]) {
    if (a.length !== b.length) {
        return false;
    }
    const intersect = a.filter(e => b.find(e1 => e1.equals(e)) !== undefined);
    return intersect.length === a.length;
}

export class DepartmentsHScrollableList extends Component<DepartmentsHScrollableListProps> {
    private employeeCards: EmployeeCardWithAvatar[];
    private scrollView: ScrollView;

    public componentDidMount() {
        InteractionManager.runAfterInteractions(() => this.setScroll());
    }
    
    public componentDidUpdate(prevProps: DepartmentsHScrollableListProps) {
        if (!arrayEquals(prevProps.departments, this.props.departments) ||
            prevProps.departmentsLists && this.props.departmentsLists &&
            prevProps.departmentsLists.currentPage !== this.props.departmentsLists.currentPage) {
                const cur = this.props.departmentsLists ? this.props.departmentsLists.currentPage : 0;
                const prev = prevProps.departmentsLists ? prevProps.departmentsLists.currentPage : 0;
                const curDep = this.props.departments[cur];
                const prevDep = prevProps.departments[prev];
                if (curDep && prevDep) {
                    if (curDep.departmentId === prevDep.departmentId && prev !== cur ||
                        curDep.departmentId !== prevDep.departmentId) {
                        this.setScroll();
                    }
                } else if (curDep && !prevDep) {
                    this.setScroll();
                }
        }
    }

    public render() {
        const { headDepartment, departments } = this.props;
        const subDepartments = headDepartment != null && departments.length > 0 ? departments : null;
        this.employeeCards = [];
        const cur = this;

        return <View>
            <ScrollView 
                horizontal 
                pagingEnabled
                showsHorizontalScrollIndicator={false}
                onMomentumScrollEnd={this.onMomentumScrollEnd}
                onScrollBeginDrag={this.onScrollBeginDrag}
                ref={this.ref}
            >
                {
                    subDepartments != null ? subDepartments.map((subDepartment, index) => 
                    <EmployeeCardWithAvatar 
                        employee={this.props.employees.employeesById.get(subDepartment.chiefId)}
                        departmentAbbreviation={subDepartment.abbreviation}
                        leftNeighbor={(index > 0) ? this.props.employees.employeesById.get(subDepartments[index - 1].chiefId) : null } 
                        rightNeighbor={(index < subDepartments.length - 1) ? this.props.employees.employeesById.get(subDepartments[index + 1].chiefId) : null} 
                        ref={(employeeCard) => { 
                            if (employeeCard != null) {
                                cur.employeeCards.push(employeeCard);
                            }
                        }} 
                        key={subDepartment.departmentId} 
                        onItemClicked={this.props.onItemClicked}
                    />) : null
                }
            </ScrollView>
        </View>;
    }

    private ref: Ref<ScrollView & Component<ScrollViewProps>> = (instance) => {
        this.scrollView = instance;
    }

    private setScroll() {
        const x: number = this.props.departmentsLists ? this.props.departmentsLists.currentPage : 0;
        const curOffsetX = Dimensions.get('window').width * x;
        if (this.scrollView) {
            this.scrollView.scrollTo({y: 0, x: curOffsetX});
        }
    }

    private onMomentumScrollEnd = (event: NativeSyntheticEvent<NativeScrollEvent>) => {
        const offset = event.nativeEvent.contentOffset;
        if (offset) {
            const currentPage = Math.round(offset.x / Dimensions.get('window').width);

            if (currentPage > this.employeeCards.length - 1) {
                this.props.updateDepartmentsBranch(this.props.headDepartment.departmentId);
            } else {
                this.employeeCards[currentPage].revealNeighboursAvatars(true);
                const visibleDepartment = this.props.departments[currentPage];
                this.props.updateDepartmentsBranch(visibleDepartment.departmentId);
            }
        }
    }

    private onScrollBeginDrag = () => {
        this.employeeCards.forEach(card => { card.revealNeighboursAvatars(false); });
    }
}