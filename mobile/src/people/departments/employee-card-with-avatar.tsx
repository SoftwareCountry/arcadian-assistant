import React, { Component } from 'react';
import { Animated, View, Dimensions, StyleSheet, TouchableOpacity } from 'react-native';
import { Avatar } from '../avatar';
import { Employee } from '../../reducers/organization/employee.model';
import { StyledText } from '../../override/styled-text';
import { companyItemStyles as styles } from '../styles';

interface EmployeeCardWithAvatarProps {
    employee: Employee;
    departmentAbbreviation: string;
    leftNeighbor?: Employee;
    rightNeighbor?: Employee;
    onItemClicked: (e: Employee) => void;
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
        Animated.timing(
          this.state.fadeInAnim,
          {
            toValue: 0.7,
            duration: 1000,
          }
        ).start();

        Animated.timing(
            this.state.fadeOutAnim,
            {
              toValue: 0,
              duration: 500,
            }
        ).start();
    }

    public render() {
        const employee = this.props.employee;
        const photo = employee ? employee.photo : null;
        const eName = employee ? employee.name : null;
        const ePosition = employee ? employee.position : null;
        const opacityValue = this.state.isNeighboursAvatarsVisible ? this.state.fadeInAnim : this.state.fadeOutAnim;

        const { layout, innerLayout, avatarContainer, avatarOuterFrame, info, name, baseText, depabbText, neighborAvatarContainer } = styles;
        const layoutFlattenStyle = StyleSheet.flatten([layout, { width: Dimensions.get('window').width }]);
        const neighborTop = ((StyleSheet.flatten(layout).height as number) - (StyleSheet.flatten(neighborAvatarContainer).height as number)) * 0.5;
        const leftNeighborX = - (StyleSheet.flatten(neighborAvatarContainer).height as number) * 0.5;
        const rightNeighborX = Dimensions.get('window').width - (StyleSheet.flatten(neighborAvatarContainer).height as number) * 0.5;
        const leftNeighborFlattenStyle = StyleSheet.flatten([neighborAvatarContainer, { top: neighborTop, left: leftNeighborX }]);
        const rightNeighborFlattenStyle = StyleSheet.flatten([neighborAvatarContainer, { top: neighborTop, left: rightNeighborX }]);

        return (
            <TouchableOpacity onPress={this.onItemClicked}>
                <Animated.View style={layoutFlattenStyle}>
                    <Animated.View style={{ ...leftNeighborFlattenStyle, opacity: opacityValue }}>
                        {this.props.leftNeighbor ? <Avatar photo={this.props.leftNeighbor.photo} /> : null}
                    </Animated.View>
                    <View style={innerLayout}>
                        <View style={avatarContainer}>
                            <Avatar photo={photo} style={avatarOuterFrame} />
                        </View>
                        <View style={info}>
                            <StyledText style={name}>{eName}</StyledText>
                            <StyledText style={baseText}>{ePosition}</StyledText>
                            <StyledText style={depabbText}>{this.props.departmentAbbreviation}</StyledText>
                        </View>
                    </View>
                    <Animated.View style={{ ...rightNeighborFlattenStyle, opacity: opacityValue }}>
                        {this.props.rightNeighbor ? <Avatar photo={this.props.rightNeighbor.photo} /> : null}
                    </Animated.View>
                </Animated.View>
            </TouchableOpacity>
        );
    }

    private onItemClicked = () => this.props.onItemClicked(this.props.employee);
}
