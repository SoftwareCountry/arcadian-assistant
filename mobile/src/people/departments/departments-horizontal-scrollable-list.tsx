import React, { Component } from 'react';
import { Animated, Easing, View, Text, ScrollView, Dimensions, TouchableOpacity, ScrollResponderEvent } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { EmployeeCardWithAvatar } from '../employee-card-with-avatar';
import { DepartmentsTree } from './departments-tree';
import { Employee } from '../../reducers/organization/employee.model';

interface DepartmentsHScrollableListProps {
    departmentsTree: DepartmentsTree;
}

export class DepartmentsHScrollableList extends Component<DepartmentsHScrollableListProps> {
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
    }

    public render() {
        /* const marginLeft = this.animatedValue.interpolate({
            inputRange: [0, 1],
            outputRange: [0, 260]
          }); */

        const employee: Employee = this.props.departmentsTree.root.head;

        return <View>
            <Animated.ScrollView 
                horizontal 
                pagingEnabled 
                onMomentumScrollEnd={function test(event: any) {
                    const eventN = event.nativeEvent.target;
                    var offset = event.nativeEvent.contentOffset;
                    if (offset) {
                        var page = Math.round(offset.x / Dimensions.get('window').width) + 1;
                        console.log('page #' + page);
                        // if (this.state.page !== page) {
                        //     console.log('page #' + page);
                        //     this.setState({ page: page });
                        // }
                    }
                    console.log('momentumScrollEnd: ' + eventN);
                }}
                onResponderRelease={function test(event: ScrollResponderEvent) { 
                    const target = event.currentTarget;
                    //const compByTarget =  ReactNativeComponentTree.getInstanceFromNode(target)._currentElement.id;
                    console.log('test on scroll' + event.currentTarget); 
                }}
            >
                <EmployeeCardWithAvatar employee={employee} ref='id1' />
                <EmployeeCardWithAvatar employee={employee} ref='id2' />
                <EmployeeCardWithAvatar employee={employee} ref='id3' />
            </Animated.ScrollView>
            {/* <TouchableOpacity onPress={this.animate} style={{ flex: 1, height: 30 }}>
                <Text ref={node => this.buttonText = node} >
                    Animate
                </Text>
            </TouchableOpacity> */}
        </View>;
    }
}