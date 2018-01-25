import React, { Component } from 'react';
import { Animated, Easing, View, Text, ScrollView, TouchableOpacity } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { EmployeeCardWithAvatar } from '../../common';


export class DepartmentsHScrollableList extends Component {
    private animatedValue: Animated.Value;
    private buttonText: Text;

    constructor() {
        super();
        this.animatedValue = new Animated.Value(0);
        this.state = { buttonText: '' };
    }

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

        return <View>
            <Animated.ScrollView horizontal >
                <EmployeeCardWithAvatar employeeName='Ivan' ref='id1' />
                <EmployeeCardWithAvatar employeeName='Petr' ref='id2' />
                <EmployeeCardWithAvatar employeeName='Sergey' ref='id3' />
            </Animated.ScrollView>
            {/* <TouchableOpacity onPress={this.animate} style={{ flex: 1, height: 30 }}>
                <Text ref={node => this.buttonText = node} >
                    Animate
                </Text>
            </TouchableOpacity> */}
        </View>;
    }
}