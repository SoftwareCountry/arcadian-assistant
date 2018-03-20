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
    employees?: Employee[];
    subordinates?: Employee[];
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
                this.props.requestEmployeesForDepartment(this.props.departmentsTree.root.departmentId);
                //this.props.requestEmployeesForDepartment('12');
            } else {
                const visibleCard: EmployeeCardWithAvatar = this.employeeCards[page - 1];
                visibleCard.revealNeighboursAvatars(true);
                this.props.updateDepartmentIdsTree(this.props.treeLevel, visibleCard.props.employee.departmentId);
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
        const isScrollEnabled = this.props.employees != null ? this.props.employees.length > 1 : false;

        return <View>
            <Animated.ScrollView 
                horizontal 
                pagingEnabled 
                scrollEnabled={isScrollEnabled} 
                onMomentumScrollEnd={this.onMomentumScrollEnd.bind(this)}
                onScrollBeginDrag={this.onScrollBeginDrag.bind(this)}
            >
                {
                    this.props.employees != null ? this.props.employees.map(employee => <EmployeeCardWithAvatar 
                        employee={employee}
                        departmentAbbreviation={this.props.departmentsTreeNodes[this.props.employees.indexOf(employee)].departmentAbbreviation} 
                        leftNeighbor={(this.props.employees.indexOf(employee) > 0 && this.props.employees.length > 1) ? this.props.employees[this.props.employees.indexOf(employee) - 1] : null } 
                        rightNeighbor={(this.props.employees.indexOf(employee) < this.props.employees.length - 1 && this.props.employees.length > 1) ? this.props.employees[this.props.employees.indexOf(employee) + 1] : null} 
                        ref={(employeeCard) => { 
                            if ( employeeCard != null ) {
                                this.employeeCards.push(employeeCard);
                            }
                        }} 
                        key={employee.employeeId} 
                    />) : null
                }
                
                {
                    // (this.props.subordinates != null && this.props.departmentsTreeNodes[0].parent != null) ? 
                    (this.props.subordinates != null) ? 
                        <EmployeeCardWithAvatar 
                            employees={this.props.subordinates} 
                            chiefId={this.props.departmentsTreeNodes != null ? this.props.departmentsTreeNodes[0].parent.chiefId : null} 
                        /> 
                            : null
                }
            </Animated.ScrollView>
        </View>;
    }
}

export const DepartmentsHScrollableList = connect(null, mapDispatchToProps)(DepartmentsHScrollableListImpl);