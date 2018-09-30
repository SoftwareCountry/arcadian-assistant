import React, { Component } from 'react';
import { View, StyleSheet, Animated, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { MapDepartmentNode } from '../reducers/people/people.model';
import { EmployeeMap } from '../reducers/organization/employees.reducer';
import { Avatar } from './avatar';
import { companyDepartmentsAnimatedNode } from './styles';

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
    public static scaleAnimation = (
        index: number, 
        width: number, 
        gap: number, 
        xCoordinate: Animated.Value
    ): ScaleAnimation => ({
        transform: [
            { perspective: 1000 },
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

	public static stickyContainerAnimation(
        index: number, 
        width: number, 
        height: number, 
        xCoordinate: Animated.Value
    ): StickyContainerAnimation {
		return {
			transform: [
                { perspective: 1000 },
				this.horizontalStickyAnimation(index, width, height, xCoordinate)
			]
		};
	}

	public static contentAnimation(
        index: number, 
        width: number, 
        height: number, 
        xCoordinate: Animated.Value
    ): ContentAnimation {
		return {
			transform: [
				{ perspective: 1000 },
				this.horizontalStickyAnimation(index, width, height, xCoordinate)
			],
			opacity: xCoordinate.interpolate({
				inputRange: [
					-width * (index + 1),
					-width * index,
					width * (1 - index)
				],
				outputRange: [-0.8, 1, 0]
			})
		};
    }
    
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
    employeesById: EmployeeMap;
    width: number;
    height: number;
    gap: number;
    xCoordinate: Animated.Value;
}

export class CompanyDepartmentsLevelNodeAnimated extends Component<CompanyDepartmentsLevelNodeAnimatedProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodeAnimatedProps) {
        return this.props.index !== nextProps.index
            || !this.props.node.equals(nextProps.node)
            || !this.props.employeesById.equals(nextProps.employeesById)
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