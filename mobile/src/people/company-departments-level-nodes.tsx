import React, { Component } from 'react';
import { DepartmentNode } from '../reducers/people/people.model';
import { Animated, PanResponder, PanResponderInstance, PanResponderGestureState, Easing, StyleSheet, View, Platform } from 'react-native';
import { companyDepartments } from './styles';
import { CompanyDepartmentsLevelAnimatedNode } from './company-departments-level-animated-node';
import { Employee } from '../reducers/organization/employee.model';
import { Optional } from 'types';

interface CompanyDepartmentsLevelNodesProps {
    width: number;
    height: number;
    nodes: DepartmentNode[];
    chiefs: Employee[];
    selectedDepartmentId: string;
    onPrevDepartment: (departmentId: string) => void;
    onNextDepartment: (departmentId: string) => void;
    onPressChief: (employee: Employee) => void;
    loadEmployeesForDepartment: (departmentId: string) => void;
}

interface CompanyDepartmentsLevelNodesState {
    xCoordinate: Animated.Value;
}

export class CompanyDepartmentsLevelNodes extends Component<CompanyDepartmentsLevelNodesProps, CompanyDepartmentsLevelNodesState> {
    private readonly motionThreshold = 20;
    private readonly gap = 100;

    private panResponder: Optional<PanResponderInstance>;

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
            onMoveShouldSetPanResponderCapture: (evt, gestureState) => {
                if (Math.abs(gestureState.dx) < 8 && Math.abs(gestureState.dy) < 8) {
                    return false;
                }
                return this.canSwipe;
            },
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
            },
            onPanResponderTerminationRequest: (e, gesture) => false,
        });
    }

    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodesProps, nextState: CompanyDepartmentsLevelNodesState) {
        const isNodesEqual = this.areNodesEqual(this.props.nodes, nextProps.nodes);
        const isChiefsEqual = this.areChiefsEqual(this.props.chiefs, nextProps.chiefs);

        return !isNodesEqual
            || !isChiefsEqual
            || this.props.selectedDepartmentId !== nextProps.selectedDepartmentId
            || this.props.height !== nextProps.height
            || this.props.width !== nextProps.width
            || this.props.onPressChief !== nextProps.onPressChief
            || this.props.loadEmployeesForDepartment !== nextProps.loadEmployeesForDepartment;
    }

    public componentDidMount() {
        this.scrollToSelectedDepartment();
        this.loadEmployeesForDepartment();
    }

    public componentDidUpdate(prevProps: CompanyDepartmentsLevelNodesProps) {
        if (this.props.selectedDepartmentId !== prevProps.selectedDepartmentId || !this.areNodesEqual(this.props.nodes, prevProps.nodes)) {
            this.state.xCoordinate.flattenOffset();
            this.scrollToSelectedDepartment();
            this.loadEmployeesForDepartment();
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
            <View style={companyDepartments.nodesContainer}>
                <Animated.View
                    {...this.panResponder!.panHandlers}
                    style={nodesContainerStyles}>
                    {
                        this.props.nodes.map((node, index) => {
                            const chief = this.props.chiefs.find(x => x.employeeId === node.chiefId);

                            return <CompanyDepartmentsLevelAnimatedNode
                                key={node.departmentId}
                                index={index}
                                node={node}
                                chief={chief}
                                width={this.props.width}
                                height={this.props.height}
                                gap={this.gap}
                                xCoordinate={this.state.xCoordinate}
                                onPressChief={this.props.onPressChief}
                            />;
                        })
                    }
                </Animated.View>
            </View>
        );
    }

    private areChiefsEqual(a: Employee[], b: Employee[]) {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        if (a.length !== b.length) {
            return false;
        }

        for (let i = 0; i < a.length; i++) {
            if (!a[i].equals(b[i])) {
                return false;
            }
        }

        return true;
    }

    private areNodesEqual(a: DepartmentNode[], b: DepartmentNode[]): boolean {
        if (a === b) {
            return true;
        }

        if (!a || !b) {
            return false;
        }

        if (a.length !== b.length) {
            return false;
        }

        for (let i = 0; i < a.length; i++) {
            if (!a[i].equals(b[i])) {
                return false;
            }
        }

        return true;
    }

    private loadEmployeesForDepartment() {
        let selectedNode = this.props.nodes.find(node => node.departmentId === this.props.selectedDepartmentId);

        if (!selectedNode) {
            selectedNode = this.props.nodes[0];
        }

        this.props.loadEmployeesForDepartment(
            selectedNode.staffDepartmentId
                ? selectedNode.staffDepartmentId
                : selectedNode.departmentId);
    }

    private scrollToSelectedDepartment() {
        let coordinate = this.gap / 2;

        if (this.props.selectedDepartmentId) {
            let index = this.props.nodes.findIndex(node => node.departmentId === this.props.selectedDepartmentId);

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
        const index = !this.props.selectedDepartmentId
            ? 0
            : this.props.nodes.findIndex(node => node.departmentId === this.props.selectedDepartmentId);

        const nextNode = this.props.nodes[index + 1];

        if (!nextNode) {
            return;
        }

        this.props.onNextDepartment(nextNode.departmentId);
    }

    private prevDepartment() {
        const index = this.props.nodes.findIndex(node => node.departmentId === this.props.selectedDepartmentId);

        const prevNode = this.props.nodes[index - 1];

        if (!prevNode) {
            return;
        }

        this.props.onPrevDepartment(prevNode.departmentId);
    }

    private isLastNextPage(): boolean {
        let index = 0;

        if (this.props.selectedDepartmentId) {
            index = this.props.nodes.findIndex(node => node.departmentId === this.props.selectedDepartmentId);
        }

        return !this.props.nodes[index + 1];
    }

    private isFirstPrevPage(): boolean {
        let index = 0;

        if (this.props.selectedDepartmentId) {
            index = this.props.nodes.findIndex(node => node.departmentId === this.props.selectedDepartmentId);
        }

        return !this.props.nodes[index - 1];
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

    private moveToNearestPage(toValue: number, onMoveComplete: Optional<() => void> = null) {
        Animated.timing(this.state.xCoordinate, {
            toValue: toValue,
            duration: 90,
            easing: Easing.linear,
            useNativeDriver: Platform.OS === 'android'
        }).start(() => {
            onMoveComplete && onMoveComplete();
            this.canSwipe = true;
        });
    }
}
