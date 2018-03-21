import React, { Component } from 'react';
import { Animated, Easing, View, Text, ScrollView, Dimensions, TouchableOpacity, ScrollResponderEvent } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { EmployeeCardWithAvatar } from '../employee-card-with-avatar';
import { DepartmentsTree } from './departments-tree';
import { DepartmentsTreeNode } from './departments-tree-node';
import { Employee } from '../../reducers/organization/employee.model';
import { PeopleActions, requestEmployeesForDepartment, updateDepartmentIdsTree } from '../../reducers/people/people.action';

interface DepartmentsHScrollableListProps {
    departmentsTree: DepartmentsTree;
    treeLevel?: number;
    departmentsTreeNodes?: DepartmentsTreeNode[];
    headDepartment: DepartmentsTreeNode;
}

interface DepartmentsHScrollableListDispatchProps {
    requestEmployeesForDepartment: (departmentId: string) => void;
    updateDepartmentIdsTree: (index: number, departmentId: string) => void;
}

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    requestEmployeesForDepartment: (departmentId: string) => { 
        dispatch(requestEmployeesForDepartment(departmentId)); 
    },
    updateDepartmentIdsTree: (index: number, departmentId: string) => { 
        dispatch(updateDepartmentIdsTree(index, departmentId)); 
    },
});

export class DepartmentsHScrollableListImpl extends Component<DepartmentsHScrollableListProps & DepartmentsHScrollableListDispatchProps> {
    private employeeCards: EmployeeCardWithAvatar[];
    private animatedValue: Animated.Value;
    private buttonText: Text;

    /* constructor() {
        super();
        this.animatedValue = new Animated.Value(0);
        this.state = { buttonText: '' };
    }
 */
/*     public animate(easing) {
        this.refs['id4'].setNativeProps({ text: 'Another Text' });
        this.animatedValue.setValue(0);
        Animated.timing(
            this.animatedValue,
            {
                toValue: 1,
                duration: 1000,
                easing
            }
        ).start();
    } */

    public componentDidMount() {
        // this.animate.bind(this, Easing.bounce);
        this.props.updateDepartmentIdsTree(this.props.treeLevel, this.props.departmentsTreeNodes[0].departmentId);
    }

    public onMomentumScrollEnd(event: any) {
        var offset = event.nativeEvent.contentOffset;
        if (offset) {
            var page = Math.round(offset.x / Dimensions.get('window').width) + 1;
            if (page > this.employeeCards.length) {
                this.props.requestEmployeesForDepartment(this.props.headDepartment.departmentId);
            } else {
                const visibleCard: EmployeeCardWithAvatar = this.employeeCards[page - 1];
                visibleCard.revealNeighboursAvatars(true);
                const visibleDepartment = this.props.headDepartment.children[page - 1];
                this.props.updateDepartmentIdsTree(this.props.treeLevel, visibleDepartment.departmentId);
            }
        }
    }

    public onScrollBeginDrag(event: any) {
        this.employeeCards.forEach(card => {
            card.revealNeighboursAvatars(false);
        });
    }

    public render() {
        this.employeeCards = [];
        
        const { headDepartment } = this.props;
        const subDepartments = headDepartment != null && headDepartment.children != null ? headDepartment.children : null;
        const subordinates = headDepartment != null && headDepartment.subordinates != null ? headDepartment.subordinates : null;

        if (subordinates != null) {
            // this.props.requestEmployeesForDepartment(this.props.headDepartment.departmentId);
        }
        
        const isScrollEnabled = subDepartments != null ? subDepartments.length > 1 : false;

        return <View>
            <Animated.ScrollView 
                horizontal 
                pagingEnabled 
                scrollEnabled={isScrollEnabled} 
                onMomentumScrollEnd={this.onMomentumScrollEnd.bind(this)}
                onScrollBeginDrag={this.onScrollBeginDrag.bind(this)}
            >
                {
                    subDepartments != null ? subDepartments.map(subDepartment => <EmployeeCardWithAvatar 
                        employee={subDepartment.head}
                        departmentAbbreviation={subDepartment.departmentAbbreviation} 
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
                    // (this.props.subordinates != null && this.props.departmentsTreeNodes[0].parent != null) ? 
                    (subordinates != null) ? 
                        <EmployeeCardWithAvatar 
                            employees={subordinates} 
                            chiefId={headDepartment.departmentChiefId} 
                        /> 
                            : null
                }
            </Animated.ScrollView>
        </View>;
    }
}

export const DepartmentsHScrollableList = connect(null, mapDispatchToProps)(DepartmentsHScrollableListImpl);