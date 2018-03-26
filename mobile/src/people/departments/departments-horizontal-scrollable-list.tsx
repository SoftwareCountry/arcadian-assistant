import React, { Component, SyntheticEvent } from 'react';
import { Animated, Easing, View, Text, ScrollView, Dimensions, TouchableOpacity, NativeScrollEvent, NativeSyntheticEvent } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { EmployeeCardWithAvatar } from '../employee-card-with-avatar';
import { DepartmentsTreeNode } from './departments-tree-node';
import { Employee } from '../../reducers/organization/employee.model';
import { PeopleActions, updateDepartmentIdsTree } from '../../reducers/people/people.action';
import { loadEmployeesForDepartment } from '../../reducers/organization/organization.action';

interface DepartmentsHScrollableListProps {
    treeLevel?: number;
    departmentsTreeNodes?: DepartmentsTreeNode[];
    headDepartment: DepartmentsTreeNode;
    topOffset?: number;
}

interface DepartmentsHScrollableListDispatchProps {
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentIdsTree: (index: number, department: DepartmentsTreeNode) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    requestEmployeesForDepartment: (departmentId: string) => { 
        dispatch(loadEmployeesForDepartment(departmentId)); 
    },
    updateDepartmentIdsTree: (index: number, department: DepartmentsTreeNode) => { 
        dispatch(updateDepartmentIdsTree(index, department)); 
    },
});

export class DepartmentsHScrollableListImpl extends Component<DepartmentsHScrollableListProps & DepartmentsHScrollableListDispatchProps> {
    private employeeCards: EmployeeCardWithAvatar[];
    private animatedValue: Animated.Value;
    private buttonText: Text;
    private currentPage: number;

    public componentDidMount() {
        // this.animate.bind(this, Easing.bounce);
        if (this.props.departmentsTreeNodes !== null) {
            this.props.requestEmployeesForDepartment(this.props.departmentsTreeNodes[0].departmentId);
        }
    }

    public onMomentumScrollEnd(event: NativeSyntheticEvent<NativeScrollEvent>) {
        var offset = event.nativeEvent.contentOffset;
        if (offset) {
            this.currentPage = Math.round(offset.x / Dimensions.get('window').width) + 1;
            if (this.currentPage > this.employeeCards.length) {
                this.props.requestEmployeesForDepartment(this.props.headDepartment.departmentId);
                // const stubDN: DepartmentsTreeNode = {
                //     departmentAbbreviation: null,
                //     departmentChiefId: this.props.headDepartment.head.employeeId,
                //     head: this.props.headDepartment.head,
                //     parent: null,
                //     children: null,
                //     subordinates: null,
                //     departmentId: 'subordinates'
                // };
                // this.props.updateDepartmentIdsTree(this.props.treeLevel, stubDN);
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

    public onScrollBeginDrag(event: NativeSyntheticEvent<NativeScrollEvent>) {
        this.employeeCards.forEach(card => {
            card.revealNeighboursAvatars(false);
        });
    }

    public render() {
        this.employeeCards = [];
        
        const { headDepartment } = this.props;
        const subDepartments = headDepartment != null && headDepartment.children != null ? headDepartment.children : null;
        const subordinates = headDepartment != null && headDepartment.subordinates != null ? headDepartment.subordinates : null;

        const isScrollEnabled = subDepartments != null ? subDepartments.length > 1 : false;

        return <View>
            <Animated.ScrollView 
                horizontal 
                pagingEnabled 
                scrollEnabled={isScrollEnabled} 
                showsHorizontalScrollIndicator={false}
                onMomentumScrollEnd={this.onMomentumScrollEnd.bind(this)}
                onScrollBeginDrag={this.onScrollBeginDrag.bind(this)}
            >
                {
                    subDepartments != null ? subDepartments.map(subDepartment => <EmployeeCardWithAvatar 
                        employee={subDepartment.head}
                        departmentAbbreviation={subDepartment.departmentAbbreviation} 
                        treeLevel={this.props.treeLevel}
                        leftNeighbor={(subDepartments.indexOf(subDepartment) > 0 && subDepartments.length > 1) ? subDepartments[subDepartments.indexOf(subDepartment) - 1].head : null } 
                        rightNeighbor={(subDepartments.indexOf(subDepartment) < subDepartments.length - 1 && subDepartments.length > 1) ? subDepartments[subDepartments.indexOf(subDepartment) + 1].head : null} 
                        ref={(employeeCard) => { 
                            if ( employeeCard != null ) {
                                this.employeeCards.push(employeeCard);
                            }
                        }} 
                        key={subDepartment.departmentId} 
                    />) : null
                }
                
                {
                    (subordinates != null) ? 
                        <EmployeeCardWithAvatar 
                            employees={subordinates} 
                            chiefId={headDepartment.departmentChiefId}
                            treeLevel={this.props.treeLevel} 
                            stretchToFitScreen={subDepartments === null || this.currentPage > subDepartments.length}
                        /> 
                            : null
                }
            </Animated.ScrollView>
        </View>;
    }
}

export const DepartmentsHScrollableList = connect(null, mapDispatchToProps)(DepartmentsHScrollableListImpl);