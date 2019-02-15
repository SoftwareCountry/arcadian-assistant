import React, { Component } from 'react';
import { Animated, Easing, Platform, StyleSheet, TouchableOpacity, View, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { DepartmentNode } from '../reducers/people/people.model';
import { companyDepartmentsAnimatedNode } from './styles';
import { Avatar } from './avatar';
import { Employee } from '../reducers/organization/employee.model';
import { Optional } from 'types';

interface Perspective {
    perspective: number;
}

interface ScaleAnimation {
    transform: (
        { scale: Animated.AnimatedInterpolation; } | Perspective
        )[];
    opacity: Animated.AnimatedInterpolation;
}

interface OpacityAnimation {
    opacity: Animated.AnimatedInterpolation;
}

interface HorizontalStickyAnimation {
    translateX: Animated.AnimatedInterpolation;
}

interface StickyContainerAnimation {
    transform: (
        HorizontalStickyAnimation | Perspective
        )[];
}

interface ContentAnimation {
    transform: (
        HorizontalStickyAnimation | Perspective
        )[];
    opacity: Animated.AnimatedInterpolation;
}

class Animations {

    private static perspective: Perspective[] = Platform.OS === 'android'
        ? [{
            // https://facebook.github.io/react-native/docs/animations#bear-in-mind:
            // without this line this Animation will not render on Android while working fine on iOS
            // ¯\_(ツ)_/¯
            perspective: 1000
        }] : [];

    public static scaleAnimation = (
        index: number,
        width: number,
        gap: number,
        xCoordinate: Animated.Value
    ): ScaleAnimation => ({
        transform: [
            ...Animations.perspective,
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
    });

    public static opacityAnimation = (
        index: number,
        width: number,
        xCoordinate: Animated.Value
    ): OpacityAnimation => ({
        opacity: xCoordinate.interpolate({
            inputRange: [
                -width * (index + 1),
                -width * index,
                width * (1 - index)
            ],
            outputRange: [0.3, 1.2, 0.3]
        })
    });

    public static stickyContainerAnimation = (
        index: number,
        width: number,
        height: number,
        xCoordinate: Animated.Value
    ): StickyContainerAnimation => ({
        transform: [
            ...Animations.perspective,
            Animations.horizontalStickyAnimation(index, width, height, xCoordinate)
        ]
    });

    public static contentAnimation = (
        index: number,
        width: number,
        height: number,
        xCoordinate: Animated.Value
    ): ContentAnimation => ({
        transform: [
            ...Animations.perspective,
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
    });

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
    });
}

interface CompanyDepartmentsLevelAnimatedNodeProps {
    index: number;
    node: DepartmentNode;
    chief: Optional<Employee>;
    width: number;
    height: number;
    gap: number;
    xCoordinate: Animated.Value;
    onPressChief: (employee: Employee) => void;
}

export class CompanyDepartmentsLevelAnimatedNode extends Component<CompanyDepartmentsLevelAnimatedNodeProps> {
    private readonly animatedContainerOpacity = new Animated.Value(0);
    private readonly isAndroid7x: boolean = false;

    constructor(props: CompanyDepartmentsLevelAnimatedNodeProps, context: any) {
        super(props, context);

        this.isAndroid7x = Platform.OS === 'android' &&
            (Platform.Version === 24 ||
             Platform.Version === 25);
    }

    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelAnimatedNodeProps) {
        return this.props.index !== nextProps.index
            || !this.props.node.equals(nextProps.node)
            || !this.isChiefSame(this.props.chief, nextProps.chief)
            || this.props.width !== nextProps.width
            || this.props.height !== nextProps.height
            || this.props.gap !== nextProps.gap
            || this.props.xCoordinate !== nextProps.xCoordinate
            || this.props.onPressChief !== nextProps.onPressChief;
    }

    public componentDidMount() {
        Animated.timing(this.animatedContainerOpacity, {
            toValue: 1,
            duration: 600,
            easing: Easing.linear,
            useNativeDriver: true
        }).start();
    }

    public render() {
        const {
            containerStyles,
            scaleContainerStyles,
            opacityContainerStyles,
            stickyContainerStyles,
            contentStyles,
            touchableStyles
        } = this.calculateStyles();

        const { chief } = this.props;
        const photo = chief ? chief.photoUrl : undefined;
        const chiefName = chief ? chief.name : null;
        const chiefPosition = chief ? chief.position : null;
        const showStaffIcon = !!this.props.node.staffDepartmentId;

        return (
            <Animated.View style={containerStyles}>
                <Animated.View style={stickyContainerStyles}>
                    <Animated.View style={this.scaleAnimationSupported() ? scaleContainerStyles : opacityContainerStyles}>
                        <TouchableOpacity style={touchableStyles} onPress={this.onPressChief}>
                            <Avatar photoUrl={photo} useDefaultForEmployeesList={showStaffIcon}/>
                        </TouchableOpacity>
                    </Animated.View>
                </Animated.View>
                <Animated.View style={contentStyles}>
                    <View>
                        <StyledText>{chiefName}</StyledText>
                        <StyledText style={companyDepartmentsAnimatedNode.contentPosition}>
                            {chiefPosition}
                        </StyledText>
                    </View>
                    <StyledText style={companyDepartmentsAnimatedNode.contentDepartmentAbbreviation}>
                        {this.props.node.abbreviation}
                    </StyledText>
                </Animated.View>
            </Animated.View>
        );
    }

    private isChiefSame(a: Optional<Employee>, b: Optional<Employee>): boolean {
        if (!a || !b) {
            return false;
        }

        if (a === b) {
            return true;
        }

        return a.equals(b);
    }

    private calculateStyles() {
        const { index, width, height, gap, xCoordinate } = this.props;
        const calculatedWidth = width - gap;

        const containerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.container,
            {
                width: calculatedWidth,
                height: height,
                opacity: this.animatedContainerOpacity as any
            }
        ]);

        const rectSize: ViewStyle = {
            width: this.scaleAnimationSupported() ? height : height * 0.9,
            height: this.scaleAnimationSupported() ? height : height * 0.9,
        };

        const scaleContainerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.scaleContainer,
            Animations.scaleAnimation(index, calculatedWidth, gap, xCoordinate) as any,
            rectSize
        ]);

        const opacityContainerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.scaleContainer,
            Animations.opacityAnimation(index, calculatedWidth, xCoordinate) as any,
            rectSize
        ]);

        const stickyContainerStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.stickyContainer,
            Animations.stickyContainerAnimation(index, calculatedWidth, height, xCoordinate) as any,
            rectSize
        ]);

        const contentStyles = StyleSheet.flatten([
            companyDepartmentsAnimatedNode.content,
            Animations.contentAnimation(index, calculatedWidth, height, xCoordinate) as any
        ]);

        const touchableStyles = StyleSheet.flatten([
            rectSize
        ]);

        return {
            containerStyles,
            scaleContainerStyles,
            opacityContainerStyles,
            stickyContainerStyles,
            contentStyles,
            touchableStyles
        };
    }

    private onPressChief = () => {
        if (this.props.chief) {
            this.props.onPressChief(this.props.chief);
        }
    };

    private scaleAnimationSupported = (): boolean => {
        return !this.isAndroid7x;
    };
}
