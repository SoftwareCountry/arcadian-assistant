import React, { Component } from 'react';
import { MapDepartmentNode, EmployeeIdToNode, DepartmentIdToSelectedId } from '../reducers/people/people.model';
import {
    View, Animated, PanResponder, PanResponderInstance, PanResponderGestureState,
    Easing, LayoutChangeEvent, StyleSheet, ViewStyle, TranslateXTransform
} from 'react-native';
import { Set } from 'immutable';
import { StyledText } from '../override/styled-text';
import { companyDepartments } from './styles';
import { CompanyDepartmentsLevelNodeAnimated } from './company-departments-level-node-animated';

interface CompanyDepartmentsLevelNodesProps {
    nodes: Set<MapDepartmentNode>;
    employeeIdToNode: EmployeeIdToNode;
    selectedDepartmentId: string;
    allowSelect: boolean;
    onPrevDepartment: (departmentId: string) => void;
    onNextDepartment: (departmentId: string) => void;
}

interface CompanyDepartmentsLevelNodesState {
    width: number;
    height: number;
    xCoordinate: Animated.Value;
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
                    this.moveToPage(gesture, -(this.state.width - this.gap), () => this.nextDepartment(), false /*this.currentPage.isPageLast*/);
                } else if (this.leftToRightSwipe(gesture)) {
                    this.canSwipe = false;
                    this.moveToPage(gesture, this.state.width - this.gap, () => this.prevDepartment(), false /*this.currentPage.isPageFirst*/);
                }
            }
        });
    }

    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodesProps, nextState: CompanyDepartmentsLevelNodesState) {
        return !this.props.nodes.equals(nextProps.nodes)
            || !this.props.employeeIdToNode.equals(nextProps.employeeIdToNode)
            || this.props.selectedDepartmentId !== nextProps.selectedDepartmentId
            || this.props.allowSelect !== nextProps.allowSelect
            || this.state.height !== nextState.height
            || this.state.width !== nextState.width;
    }

    public render() {
        this.scrollToSelectedDepartment();

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
                                const chiefId = node.get('chiefId');
                                const chief = this.props.employeeIdToNode.get(chiefId);

                                return <CompanyDepartmentsLevelNodeAnimated
                                    key={node.get('departmentId')}
                                    index={index}                                    
                                    node={node}
                                    chief={chief}                                    
                                    width={this.state.width}
                                    height={this.state.height}
                                    gap={this.gap}
                                    xCoordinate={this.state.xCoordinate}
                                />;
                            })
                    }
                </Animated.View>
            </View>
        );
    }

    private scrollToSelectedDepartment() {
        if (!this.props.allowSelect) {
            return;
        }

        const index = this.props.nodes.toArray().findIndex(node => node.get('departmentId') === this.props.selectedDepartmentId);

        if (index === -1) {
            return;
        }

        this.state.xCoordinate.setOffset(-(this.state.width - this.gap) * index);
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

    private nextDepartment() {
        const nodesArray = this.props.nodes.toArray();
        const index = nodesArray.findIndex(node => node.get('departmentId') === this.props.selectedDepartmentId);

        const nextNode = nodesArray[index + 1];

        if (!nextNode) {
            return;
        }

        this.props.onNextDepartment(nextNode.get('departmentId'));
    }

    private prevDepartment() {
        const nodesArray = this.props.nodes.toArray();
        const index = nodesArray.findIndex(node => node.get('departmentId') === this.props.selectedDepartmentId);

        const prevNode = nodesArray[index - 1];

        if (!prevNode) {
            return;
        }        
        
        this.props.onPrevDepartment(prevNode.get('departmentId'));
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