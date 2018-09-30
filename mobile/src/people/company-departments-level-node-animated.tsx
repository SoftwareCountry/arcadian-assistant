import React, { Component } from 'react';
import { View, StyleSheet, Animated, ViewStyle, PerpectiveTransform } from 'react-native';
import { StyledText } from '../override/styled-text';
import { MapDepartmentNode, EmployeeIdToNode } from '../reducers/people/people.model';
import { companyDepartmentsAnimatedNode } from './styles';
import { Map } from 'immutable';
import { Photo } from '../reducers/organization/employee.model';
import { CompanyDepartmentsLevelNodePhoto } from './company-departments-employee-node';

interface Perspective {
    perspective: number;
}

interface ScaleAnimation {
    transform: [
        Perspective,
        { scale: Animated.AnimatedInterpolation; }
    ];
    opacity: Animated.AnimatedInterpolation;
}

interface HorizontalStickyAnimation {
    translateX: Animated.AnimatedInterpolation;
}

interface StickyContainerAnimation {
    transform: [
        Perspective,
        HorizontalStickyAnimation
    ];
}

interface ContentAnimation {
    transform: [
        Perspective, 
        HorizontalStickyAnimation
    ];
    opacity: Animated.AnimatedInterpolation;
}

class Animations {

    private static perspective: PerpectiveTransform = { 
        // https://facebook.github.io/react-native/docs/animations#bear-in-mind:
        // without this line this Animation will not render on Android while working fine on iOS
        // ¯\_(ツ)_/¯        
        perspective: 1000 
    };

    public static scaleAnimation = (
        index: number, 
        width: number, 
        gap: number, 
        xCoordinate: Animated.Value
    ): ScaleAnimation => ({
        transform: [
            Animations.perspective,
            {
                scale: xCoordinate.interpolate({
                    inputRange: [
                        -width * (index + 1),
                        -width * index,
                        width * (1 - index) + gap
                    ],
                    outputRange: [0.6, 0.9, 0.6]
                })
            }
        ],
        opacity: xCoordinate.interpolate({
            inputRange: [
                -width * (index + 1),
                -width * index,
                width * (1 - index)
            ],
            outputRange: [0.3, 1.2, 0.3]
        })
    })

	public static stickyContainerAnimation = (
        index: number, 
        width: number, 
        height: number, 
        xCoordinate: Animated.Value
    ): StickyContainerAnimation => ({
        transform: [
            Animations.perspective,
            Animations.horizontalStickyAnimation(index, width, height, xCoordinate)
        ]
	})

	public static contentAnimation = (
        index: number, 
        width: number, 
        height: number, 
        xCoordinate: Animated.Value
    ): ContentAnimation => ({
        transform: [
            Animations.perspective,
            Animations.horizontalStickyAnimation(index, width, height, xCoordinate)
        ],
        opacity: xCoordinate.interpolate({
            inputRange: [
                -width * (index + 1),
                -width * index,
                width * (1 - index)
            ],
            outputRange: [-0.8, 1, 0]
        })
    })

	private static horizontalStickyAnimation = (
        index: number, 
        width: number, 
        height: number, 
        xCoordinate: Animated.Value
    ): HorizontalStickyAnimation => ({
        translateX: xCoordinate.interpolate({
            inputRange: [
                -width * (index + 1),
                -width * (index + 1) + height / 2,
                -width * index,
                width * (1 - index)
            ],
            outputRange: [width - height, width - height, 0, 0]
        })
    })    
}

interface CompanyDepartmentsLevelNodeAnimatedProps {
    index: number;
    node: MapDepartmentNode;
    employeeIdToNode: EmployeeIdToNode;
    width: number;
    height: number;
    gap: number;
    xCoordinate: Animated.Value;
}

export class CompanyDepartmentsLevelNodeAnimated extends Component<CompanyDepartmentsLevelNodeAnimatedProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodeAnimatedProps) {
        return this.props.index !== nextProps.index
            || !this.props.node.equals(nextProps.node)
            || !this.props.employeeIdToNode.equals(nextProps.employeeIdToNode)
            || this.props.width !== nextProps.width
            || this.props.height !== nextProps.height
            || this.props.gap !== nextProps.gap
            || this.props.xCoordinate !== nextProps.xCoordinate;
    }

    public render() {
        const { 
            containerStyles, 
            stickyContainerStyles, 
            scaleContainerStyles, 
            contentStyles 
        } = this.calculateStyles();

        const chiefId = this.props.node.get('chiefId');
        const chief = this.props.employeeIdToNode.get(chiefId);

        return chief ?
            <View style={containerStyles}>
                <Animated.View style={stickyContainerStyles}>
                    <Animated.View style={scaleContainerStyles}>
                        <CompanyDepartmentsLevelNodePhoto photo={chief.get('photo') as Map<string, Photo>} />
                    </Animated.View>
                </Animated.View>
                <Animated.View style={contentStyles}>
                    <View>
                        <StyledText>{chief.get('name')}</StyledText>
                        <StyledText>{chief.get('position')}</StyledText>
                    </View>
                    <StyledText>{this.props.node.get('abbreviation')}</StyledText>
                </Animated.View>
            </View> : null;
    }

    private calculateStyles() {
        const { index, width, height, gap, xCoordinate } = this.props;
        const calculatedWidth = width - gap;

        const containerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.container,
            {
                width: calculatedWidth,
                height: height
            }
        ]);

        const rectSize: ViewStyle = {
            width: height,
            height: height
        };

        const scaleContainerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.scaleContainer,
            Animations.scaleAnimation(index, calculatedWidth, gap, xCoordinate) as any,
            rectSize
        ]);

        const stickyContainerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.stickyContainer,
            Animations.stickyContainerAnimation(index, calculatedWidth, height, xCoordinate)  as any,
            rectSize
        ]);

        const contentStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.content,
            Animations.contentAnimation(index, calculatedWidth, height, xCoordinate) as any
        ]);

        return {
            containerStyles,
            scaleContainerStyles,
            stickyContainerStyles,
            contentStyles
        };
    }
}