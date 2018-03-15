import React, { Component } from 'react';
import { Animated, Easing, View, Image, Dimensions, StyleSheet } from 'react-native';
import { Avatar } from './avatar';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';
import { companyItemStyles as styles } from './styles';

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
            <Animated.View style={{ width: Dimensions.get('window').width, height: 90, overflow: 'hidden' }}>
                <Animated.View style={{ position: 'absolute', top: (90 - 44) * 0.5, left: -44 * 0.5, opacity: neighboursAvatarsVisibility ? fadeInAnim : fadeOutAnim }}>
                    { this.props.leftNeighbor ? <Avatar photo={this.props.leftNeighbor.photo} style={{ width: 44, height: 44 }} /> : null }
                </Animated.View>
                <View style={styles.layout}>
                    <View style={styles.avatarContainer}>
                        <Avatar photo={photo} style={{ width: 56, height: 56 }} />
                    </View>
                    <View style={styles.info}>
                        <StyledText style={styles.name}>{employee.name}</StyledText>
                        <StyledText style={styles.baseText}>{employee.position}</StyledText>
                        <StyledText style={styles.depabbText}>DEP_ABB</StyledText>
                    </View>
                </View>
                <Animated.View style={{ position: 'absolute', top: (90 - 44) * 0.5, left: (Dimensions.get('window').width - 44 * 0.5 ), opacity: neighboursAvatarsVisibility ? fadeInAnim : fadeOutAnim}}>
                    { this.props.rightNeighbor ? <Avatar photo={this.props.rightNeighbor.photo} style={{ width: 44, height: 44 }} /> : null }
                </Animated.View>
            </Animated.View>
        );
    }
}