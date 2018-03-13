import React, { Component } from 'react';
import { Animated, Easing, View, Image, Dimensions, StyleSheet } from 'react-native';
import { Avatar } from './avatar';
import { Employee } from '../reducers/organization/employee.model';

interface EmployeeCardWithAvatarProps {
    employee: Employee;
    leftNeighbor: Employee;
    rightNeighbor: Employee;
}

export class EmployeeCardWithAvatar extends Component<EmployeeCardWithAvatarProps> {
    public state = {
        fadeInAnim: new Animated.Value(0),
        fadeOutAnim: new Animated.Value(1)
    };

    public revealNeighborsAvatars = () => {
        console.log('pop up neighbors');
    }

    public componentDidMount() {
        Animated.timing(
          this.state.fadeInAnim,
          {
            toValue: 1,
            duration: 2000,
          }
        ).start();

        Animated.timing(
            this.state.fadeOutAnim,
            {
              toValue: 0,
              duration: 1000,
            }
        ).start();
    }

    public render() {
        const employee = this.props.employee;
        const { fadeInAnim, fadeOutAnim } = this.state;
        const photo = employee ? employee.photo : null;

        return (
            <Animated.View style={{ width: Dimensions.get('window').width, height: 100, justifyContent: 'space-between', opacity: fadeInAnim, overflow: 'hidden' }}>
                <View style={{ position: 'absolute', top: (100 - 50) * 0.5, left: -50 * 0.5 }}>
                    { this.props.leftNeighbor ? <Avatar photo={this.props.leftNeighbor.photo} style={{ width: 50, height: 50 }} /> : null }
                </View>
                <Avatar photo={photo} style={{ width: 150, height: 150 }} />
                <View style={{ position: 'absolute', top: (100 - 50) * 0.5, left: (Dimensions.get('window').width - 50 * 0.5 )}}>
                    { this.props.rightNeighbor ? <Avatar photo={this.props.rightNeighbor.photo} style={{ width: 50, height: 50 }} /> : null }
                </View>
            </Animated.View>
        );
    }

    public revealNeighbors() {
        console.log('Reveal Neighbors photo if any');
    }
}