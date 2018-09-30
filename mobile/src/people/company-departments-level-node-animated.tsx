import React, { Component } from 'react';
import { View, StyleSheet, Animated, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { MapDepartmentNode } from '../reducers/people/people.model';
import { EmployeeMap } from '../reducers/organization/employees.reducer';
import { Avatar } from './avatar';
import { companyDepartmentsAnimatedNode } from './styles';

export interface Perspective {
    perspective: number;
}

export interface ScaleAnimation {
    transform: [
        Perspective,
        { scale: Animated.AnimatedInterpolation; }
    ];
    opacity: Animated.AnimatedInterpolation;
}

export interface HorizontalStickyAnimation {
    translateX: Animated.AnimatedInterpolation;
}

export interface StickyContainerAnimation {
    transform: [
        Perspective,
        HorizontalStickyAnimation
    ];
}

export interface ContentAnimation {
    transform: [
        Perspective, 
        HorizontalStickyAnimation
    ];
    opacity: Animated.AnimatedInterpolation;
}

interface CompanyDepartmentsLevelNodeAnimatedProps {
    node: MapDepartmentNode;
    employeesById: EmployeeMap;
    width: number;
    height: number;
    scaleAnimation: ScaleAnimation;
    stickyContainerAnimation: StickyContainerAnimation;
    contentAnimation: ContentAnimation;
}

export class CompanyDepartmentsLevelNodeAnimated extends Component<CompanyDepartmentsLevelNodeAnimatedProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodeAnimatedProps) {
        return !this.props.node.equals(nextProps.node)
            || !this.props.employeesById.equals(nextProps.employeesById)
            || this.props.width !== nextProps.width
            || this.props.height !== nextProps.height
            || this.props.scaleAnimation !== nextProps.scaleAnimation
            || this.props.stickyContainerAnimation !== nextProps.stickyContainerAnimation
            || this.props.contentAnimation !== nextProps.contentAnimation;
    }

    public render() {
        const containerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.container,
            {
                width: this.props.width,
                height: this.props.height
            }
        ]);

        const scaleContainerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.scaleContainer as any,
            this.props.scaleAnimation,
            {
                width: this.props.width,
                height: this.props.height
            }
        ]);

        const stickyContainerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.stickyContainer as any,
            this.props.stickyContainerAnimation,
            {
                height: this.props.height,
                width: this.props.height,
            }
        ]);

        const contentStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.content as any,
            this.props.contentAnimation
        ]);

        const chiefId = this.props.node.get('chiefId');
        const chief = this.props.employeesById.get(chiefId);

        return chief ?
            <View style={containerStyles}>
                <Animated.View style={stickyContainerStyles}>
                    <Animated.View style={scaleContainerStyles}>
                        <Avatar photo={chief.photo} />
                    </Animated.View>
                </Animated.View>
                <Animated.View style={contentStyles}>
                    <View>
                        <StyledText>{chief.name}</StyledText>
                        <StyledText>{chief.position}</StyledText>
                    </View>
                    <StyledText>{this.props.node.get('abbreviation')}</StyledText>
                </Animated.View>
            </View> : null;
    }
}