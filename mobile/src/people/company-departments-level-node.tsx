import React, { Component } from 'react';
import { MapDepartmentNode } from '../reducers/people/people.model';
import {
    View, Animated, PanResponder, PanResponderInstance, PanResponderGestureState,
    Easing, LayoutChangeEvent, StyleSheet, ViewStyle, TranslateXTransform
} from 'react-native';
import { Set } from 'immutable';
import { StyledText } from '../override/styled-text';
import { companyDepartments } from './styles';
import { CompanyDepartmentsLevelNodeAnimated } from './company-departments-level-node-animated';
import { EmployeeMap } from '../reducers/organization/employees.reducer';

interface CompanyDepartmentsLevelNodesProps {
    nodes: Set<MapDepartmentNode>;
    employeesById: EmployeeMap;
}

interface CompanyDepartmentsLevelNodesState {
    width: number;
    height: number;
    xCoordinate: Animated.Value;
}

interface AnimatedTranslateX {
    translateX: Animated.Value;
}

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

export class CompanyDepartmentsLevelNodes extends Component<CompanyDepartmentsLevelNodesProps, CompanyDepartmentsLevelNodesState> {
    private readonly motionThreshold = 20;
    private readonly gap = 100;

    private panResponder: PanResponderInstance;

    // Why isn't in state? Answer: Perfomance reason.
    // Changing the state.canSwipe makes twise render calls (when swipe is started, and swipe is ended), it impacts on the animation smoothness,
    // causing some flickers
    private canSwipe = true;

    constructor(props: CompanyDepartmentsLevelNodesProps) {
        super(props);
        this.state = {
            width: this.gap,
            height: 0,
            xCoordinate: new Animated.Value( this.gap / 2 )
        };
    }

    public componentWillMount() {
        this.panResponder = PanResponder.create({
            onMoveShouldSetPanResponderCapture: (evt, gestureState) => this.canSwipe,
            onPanResponderMove: Animated.event(
                [
                    null,
                    { dx: this.state.xCoordinate }
                ]
            ),
            onPanResponderGrant: (e, gesture) => {
                this.state.xCoordinate.extractOffset();
            },
            onPanResponderRelease: (e, gesture) => {
                if (this.rightToLeftSwipe(gesture)) {
                    this.canSwipe = false;
                    this.moveToPage(gesture, -(this.state.width - this.gap), () => this.nextPage(), false /*this.currentPage.isPageLast*/);
                } else if (this.leftToRightSwipe(gesture)) {
                    this.canSwipe = false;
                    this.moveToPage(gesture, this.state.width - this.gap, () => this.prevPage(), false /*this.currentPage.isPageFirst*/);
                }
            }
        });
    }

    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodesProps, nextState: CompanyDepartmentsLevelNodesState) {
        return !this.props.nodes.equals(nextProps.nodes)
            || this.state.height !== nextState.height
            || this.state.width !== nextState.width;
    }

    public render() {
        const nodesContainerStyles = StyleSheet.flatten([
            companyDepartments.nodesSwipeableContainer,
            {
                transform: [{translateX: this.state.xCoordinate as any}]
            }
        ]);

        return (
            <View style={companyDepartments.nodesContainer} onLayout={this.onLayoutContainer}>
                <Animated.View
                    {...this.panResponder.panHandlers}
                    style={nodesContainerStyles}>
                    {
                        this.props.nodes.toArray()
                            .map((node, index) => {
                                const scaleAnimation = this.scaleAnimation(index);
                                const stickyContainerAnimation = this.stickyContainerAnimation(index);
                                const contentAnimation = this.contentAnimation(index);

                                return <CompanyDepartmentsLevelNodeAnimated
                                    key={node.get('departmentId')}
                                    node={node}
                                    width={this.state.width - this.gap}
                                    height={this.state.height}
                                    employeesById={this.props.employeesById}
                                    scaleAnimation={scaleAnimation}
                                    stickyContainerAnimation={stickyContainerAnimation}
                                    contentAnimation={contentAnimation}
                                />;
                            })
                    }
                </Animated.View>
            </View>
        );
    }

    private scaleAnimation(index: number): ScaleAnimation {
        const width = this.state.width - this.gap;

        return {
            transform: [
                { perspective: 1000 },
                {
                    scale: this.state.xCoordinate.interpolate({
                        inputRange: [
                            -width * (index + 1),
                            -width * index,
                            width * (1 - index) + this.gap
                        ],
                        outputRange: [0.6, 0.9, 0.6]
                    })
                }
            ],
            opacity: this.state.xCoordinate.interpolate({
                inputRange: [
                    -width * (index + 1),
                    -width * index,
                    width * (1 - index)
                ],
                outputRange: [0.3, 1.2, 0.3]
            })
        };
    }

	private horizontalStickyAnimation(index: number, width: number): HorizontalStickyAnimation {
		return {
			translateX: this.state.xCoordinate.interpolate({
				inputRange: [
					-width * (index + 1),
					-width * (index + 1) + this.state.height / 2,
					-width * index,
					width * (1 - index)
				],
				outputRange: [width - this.state.height, width - this.state.height, 0, 0]
			})
		};
    }

	private stickyContainerAnimation(index: number): StickyContainerAnimation {
		const width = this.state.width - this.gap;
		return {
			transform: [
				{ perspective: 1000 },
				this.horizontalStickyAnimation(index, width)
			]
		};
	}

	private contentAnimation(index: number): ContentAnimation {
		const width = this.state.width - this.gap;
		return {
			transform: [
				{ perspective: 1000 },
				this.horizontalStickyAnimation(index, width)
			],
			opacity: this.state.xCoordinate.interpolate({
				inputRange: [
					-width * (index + 1),
					-width * index,
					width * (1 - index)
				],
				outputRange: [-0.8, 1, 0]
			})
		};
	}

    private onLayoutContainer = (e: LayoutChangeEvent) => {
        this.setState({
            width: e.nativeEvent.layout.width,
            height: e.nativeEvent.layout.height
        });
    }

    private rightToLeftSwipe(gesture: PanResponderGestureState): boolean {
        return gesture.dx < 0;
    }

    private leftToRightSwipe(gesture: PanResponderGestureState): boolean {
        return gesture.dx > 0;
    }

    private nextPage() {
        //this.props.onNextPage();
    }

    private prevPage() {
        //this.props.onPrevPage();
    }

    // private currentPage() {
    //     return this.props.pages[1];
    // }

    private moveToPage(
        gesture: PanResponderGestureState,
        toValue: number,
        onCompleteMove: () => void,
        isFirstOrLast: boolean
    ) {
        if (this.isThresholdExceeded(gesture) && !isFirstOrLast) {
            this.moveToNearestPage(toValue, onCompleteMove);
        } else {
            this.moveToNearestPage(0, onCompleteMove);
        }
    }

    private isThresholdExceeded(gesture: PanResponderGestureState): boolean {
        return this.motionThreshold - Math.abs(gesture.dx) <= 0;
    }

    private moveToNearestPage(toValue: number, onMoveComplete: () => void) {
        Animated.timing(this.state.xCoordinate, {
            toValue: toValue,
            duration: 90,
            easing: Easing.linear,
            useNativeDriver: true
        }).start(() => {
            onMoveComplete();
            this.canSwipe = true;
        });
    }
}