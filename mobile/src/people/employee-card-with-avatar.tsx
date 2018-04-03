import React, { Component } from 'react';
import { Animated, Easing, View, Image, Dimensions, StyleSheet, FlatList, ListRenderItemInfo, TouchableOpacity } from 'react-native';
import { Avatar } from './avatar';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';
import { companyItemStyles as styles, companyTinyItemStyles as tinyStyles} from './styles';

interface EmployeeCardWithAvatarProps {
    employee?: Employee;
    departmentAbbreviation?: string;
    chiefId?: string;
    employees?: Employee[];
    leftNeighbor?: Employee;
    rightNeighbor?: Employee;
    treeLevel?: number;
    stretchToFitScreen?: boolean;
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
        const employees = this.props.employees;
        
        if (employees != null) {
            const filteredEmployees = employees.filter((emp) => emp.employeeId !== this.props.chiefId);
            return this.lowestLevelEmployeesList(filteredEmployees);
        } else {
            return this.standardEmployeeCard();
        }
    }

    private standardEmployeeCard = () => {
        const employee = this.props.employee;
        const photo = employee ? employee.photo : null;
        const { fadeInAnim, fadeOutAnim } = this.state;
        const neighboursAvatarsVisibility = this.state.isNeighboursAvatarsVisible;
        const {layout, innerLayout, avatarContainer, avatarOuterFrame, avatarImage, info, name, baseText, depabbText, neighborAvatarContainer, neighborAvatarImage } = styles;

        const layoutFlattenStyle = StyleSheet.flatten([layout, {width: Dimensions.get('window').width}]);

        const neighborTop = ((StyleSheet.flatten(layout).height as number) - (StyleSheet.flatten(neighborAvatarContainer).height as number)) * 0.5;
        const leftNeighborX = - (StyleSheet.flatten(neighborAvatarContainer).height as number) * 0.5;
        const rightNeighborX = Dimensions.get('window').width - (StyleSheet.flatten(neighborAvatarContainer).height as number) * 0.5;
        const opacityValue = neighboursAvatarsVisibility ? fadeInAnim : fadeOutAnim;

        const leftNeighborFlattenStyle = StyleSheet.flatten([neighborAvatarContainer, {top: neighborTop, left: leftNeighborX}]);
        const rightNeighborFlattenStyle = StyleSheet.flatten([neighborAvatarContainer, {top: neighborTop, left: rightNeighborX}]);

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
                            <StyledText style={name}>{employee.name}</StyledText>
                            <StyledText style={baseText}>{employee.position}</StyledText>
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

    private lowestLevelEmployeesList = (employees: Employee[]) => {
        const calcultatedHeight = this.props.stretchToFitScreen ? Dimensions.get('screen').height - 90 * this.props.treeLevel - 119 : 90;
        const { layout } = styles;
        const layoutFlattenStyle = StyleSheet.flatten([
            layout, {width: Dimensions.get('window').width, height: calcultatedHeight}
        ]);
        
        return (
            <View style={layoutFlattenStyle}>
                <FlatList
                    data={employees}
                    keyExtractor={this.keyExtractor}
                    renderItem={this.renderItem} />
            </View>
        );
    }

    private keyExtractor = (item: Employee) => item.employeeId;

    private renderItem = (itemInfo: ListRenderItemInfo<Employee>) => {
        const { item } = itemInfo;
        const { innerLayout, avatarContainer, avatarOuterFrame, avatarImage, info, name, baseText } = tinyStyles;
        return (
            <TouchableOpacity onPress={() => this.onLowestLevelItemClicked(itemInfo.index)}>
                <View style={innerLayout} key={item.employeeId}>
                    <View style={avatarContainer}>
                        <Avatar photo={item.photo} style={avatarOuterFrame} imageStyle={avatarImage} />
                    </View>
                    <View style={info}>
                        <StyledText style={name}>{item.name}, </StyledText>
                        <StyledText style={baseText}>{item.position}</StyledText>
                    </View>
                </View>
            </TouchableOpacity>
        );
    }

    private onItemClicked = () => this.props.onItemClicked(this.props.employee);
    private onLowestLevelItemClicked = (itemId: number) => {
        const filteredEmployees = this.props.employees.filter((emp) => emp.employeeId !== this.props.chiefId);
        this.props.onItemClicked(filteredEmployees[itemId]);
    }
}
