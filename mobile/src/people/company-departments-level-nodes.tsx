import React, { Component } from 'react';
import { MapDepartmentNode, EmployeeIdToNode, DepartmentIdToSelectedId, MapEmployeeNode } from '../reducers/people/people.model';
import { Animated, PanResponder, PanResponderInstance, PanResponderGestureState, Easing, StyleSheet } from 'react-native';
import { Set } from 'immutable';
import { StyledText } from '../override/styled-text';
import { companyDepartments } from './styles';
import { CompanyDepartmentsLevelAnimatedNode } from './company-departments-level-animated-node';

interface CompanyDepartmentsLevelNodesProps {
    width: number;
    height: number;    
    nodes: Set<MapDepartmentNode>;
    chiefs: EmployeeIdToNode;
    selectedDepartmentId: string;
    onPrevDepartment: (departmentId: string) => void;
    onNextDepartment: (departmentId: string) => void;
}

interface CompanyDepartmentsLevelNodesState {
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
            xCoordinate: new Animated.Value(0)
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
                    this.moveToPage(gesture, -(this.props.width - this.gap), () => this.nextDepartment(), this.isLastNextPage());
                } else if (this.leftToRightSwipe(gesture)) {
                    this.canSwipe = false;
                    this.moveToPage(gesture, this.props.width - this.gap, () => this.prevDepartment(), this.isFirstPrevPage());
                }
            }
        });
    }

    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodesProps, nextState: CompanyDepartmentsLevelNodesState) {
        return !this.props.nodes.equals(nextProps.nodes)
            || !this.props.chiefs.equals(nextProps.chiefs)
            || this.props.selectedDepartmentId !== nextProps.selectedDepartmentId
            || this.props.height !== nextProps.height
            || this.props.width !== nextProps.width;
    }

    public componentDidMount() {
        this.scrollToSelectedDepartment();
    }

    public componentDidUpdate(prevProps: CompanyDepartmentsLevelNodesProps) {
        if (this.props.selectedDepartmentId !== prevProps.selectedDepartmentId || this.props.nodes.size !== prevProps.nodes.size) {
            this.state.xCoordinate.flattenOffset();
            this.scrollToSelectedDepartment();
        }
    }

    public render() {

        const nodesContainerStyles = StyleSheet.flatten([
            companyDepartments.nodesSwipeableContainer,
            {
                transform: [{ translateX: this.state.xCoordinate as any }]
            }
        ]);

        return (
            <Animated.View
                {...this.panResponder.panHandlers}
                style={nodesContainerStyles}>
                {
                    this.props.nodes.toArray()
                        .map((node, index) => {
                            const chiefId = node.get('chiefId');
                            const chief = this.props.chiefs.get(chiefId);

                            return <CompanyDepartmentsLevelAnimatedNode
                                key={node.get('departmentId')}
                                index={index}                                    
                                node={node}
                                chief={chief}                                    
                                width={this.props.width}
                                height={this.props.height}
                                gap={this.gap}
                                xCoordinate={this.state.xCoordinate}
                            />;
                        })
                }
            </Animated.View>
        );
    }

    private scrollToSelectedDepartment() {
        let coordinate = this.gap / 2;

        if (this.props.selectedDepartmentId) {
            let index = this.props.nodes.toArray().findIndex(node => node.get('departmentId') === this.props.selectedDepartmentId);
            
            if (index === -1) {
                return;
            }

            coordinate += -(this.props.width - this.gap) * index;
        }

        this.state.xCoordinate.setValue(coordinate);
    }

    private rightToLeftSwipe(gesture: PanResponderGestureState): boolean {
        return gesture.dx < 0;
    }

    private leftToRightSwipe(gesture: PanResponderGestureState): boolean {
        return gesture.dx > 0;
    }

    private nextDepartment() {
        const nodesArray = this.props.nodes.toArray();
        const index = !this.props.selectedDepartmentId 
            ? 0
            : nodesArray.findIndex(node => node.get('departmentId') === this.props.selectedDepartmentId);

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

    private isLastNextPage(): boolean {
        const nodesArray = this.props.nodes.toArray();

        let index = 0;

        if (this.props.selectedDepartmentId) {
            index = nodesArray.findIndex(node => node.get('departmentId') === this.props.selectedDepartmentId);
        }
        
        return !nodesArray[index + 1];
    }

    private isFirstPrevPage(): boolean {
        const nodesArray = this.props.nodes.toArray();

        let index = 0;

        if (this.props.selectedDepartmentId) {
            index = nodesArray.findIndex(node => node.get('departmentId') === this.props.selectedDepartmentId);
        }
        
        return !nodesArray[index - 1];        
    }

    private moveToPage(
        gesture: PanResponderGestureState,
        toValue: number,
        onCompleteMove: () => void,
        isFirstOrLast: boolean
    ) {
        if (this.isThresholdExceeded(gesture) && !isFirstOrLast) {
            this.moveToNearestPage(toValue, onCompleteMove);
        } else {
            this.moveToNearestPage(0);
        }
    }

    private isThresholdExceeded(gesture: PanResponderGestureState): boolean {
        return this.motionThreshold - Math.abs(gesture.dx) <= 0;
    }

    private moveToNearestPage(toValue: number, onMoveComplete: () => void = null) {
        Animated.timing(this.state.xCoordinate, {
            toValue: toValue,
            duration: 90,
            easing: Easing.linear,
            useNativeDriver: true
        }).start(() => {
            onMoveComplete && onMoveComplete();
            this.canSwipe = true;
        });
    }
}