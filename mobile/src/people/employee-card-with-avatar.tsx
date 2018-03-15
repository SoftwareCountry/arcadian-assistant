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
        fadeOutAnim: new Animated.Value(1),
        isNeighboursAvatarsVisible: true
    };

    public revealNeighboursAvatars(visibility: boolean) {
        this.setState({isNeighboursAvatarsVisible: visibility});
    }

    public componentDidMount() {
        // Animated.timing(
        //   this.state.fadeInAnim,
        //   {
        //     toValue: 1,
        //     duration: 3000,
        //   }
        // ).start();

        // Animated.timing(
        //     this.state.fadeOutAnim,
        //     {
        //       toValue: 0,
        //       duration: 2000,
        //     }
        // ).start();
    }

    public render() {
        const employee = this.props.employee;
        const neighboursAvatarsVisibility = this.state.isNeighboursAvatarsVisible;
        const { fadeInAnim, fadeOutAnim } = this.state;
        const photo = employee ? employee.photo : null;

        Animated.timing(
            this.state.fadeInAnim,
            {
              toValue: 1,
              duration: 3000,
            }
          ).start();
  
        Animated.timing(
              this.state.fadeOutAnim,
              {
                toValue: 0,
                duration: 2000,
              }
          ).start();
  
        return (
            <Animated.View style={{ width: Dimensions.get('window').width, height: 100, justifyContent: 'space-between', overflow: 'hidden' }}>
                <Animated.View style={{ position: 'absolute', top: (100 - 50) * 0.5, left: -50 * 0.5, opacity: neighboursAvatarsVisibility ? fadeInAnim : fadeOutAnim }}>
                    { this.props.leftNeighbor ? <Avatar photo={this.props.leftNeighbor.photo} style={{ width: 50, height: 50 }} /> : null }
                </Animated.View>
                <Avatar photo={photo} style={{ width: 150, height: 150 }} />
                <Animated.View style={{ position: 'absolute', top: (100 - 50) * 0.5, left: (Dimensions.get('window').width - 50 * 0.5 ), opacity: neighboursAvatarsVisibility ? fadeInAnim : fadeOutAnim}}>
                    { this.props.rightNeighbor ? <Avatar photo={this.props.rightNeighbor.photo} style={{ width: 50, height: 50 }} /> : null }
                </Animated.View>
            </Animated.View>
        );
    }
}