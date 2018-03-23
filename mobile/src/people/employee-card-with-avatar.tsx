import React, { Component } from 'react';
import { Animated, Easing, View, Image, Dimensions, StyleSheet, FlatList, ListRenderItemInfo } from 'react-native';
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
        // Animated.timing(
        //   this.state.fadeInAnim,
        //   {
        //     toValue: 1,
        //     duration: 3000,
        //   }
        // ).start();

        // Animated.timing(
        //     this.state.fadeOutAnim,
        //     {
        //       toValue: 0,
        //       duration: 2000,
        //     }
        // ).start();
    }

    public render() {
        const employee = this.props.employee;
        const neighboursAvatarsVisibility = this.state.isNeighboursAvatarsVisible;
        const { fadeInAnim, fadeOutAnim } = this.state;
        const photo = employee ? employee.photo : null;

        Animated.timing(
            this.state.fadeInAnim,
            {
              toValue: 1,
              duration: 1000,
            }
          ).start();
  
        Animated.timing(
              this.state.fadeOutAnim,
              {
                toValue: 0,
                duration: 1000,
              }
          ).start();
  
        const employees = this.props.employees;
        
        if (employees != null) {
            const calcultatedHeight =  this.props.stretchToFitScreen ? Dimensions.get('screen').height - 90 * this.props.treeLevel - 119 : 90; 
            // console.log('Tree level: ' + this.props.treeLevel + ' Calculated height: ' + calcultatedHeight);
            const { layout } = styles;
            const layoutFlattenStyle = StyleSheet.flatten([
                layout,
                {
                    width: Dimensions.get('window').width,
                    height: calcultatedHeight,
                    backgroundColor: '#abfcba'
                }
            ]);
            const filteredEmployees = employees.filter((emp) => emp.employeeId !== this.props.chiefId);
            return (
                <View style={layoutFlattenStyle}>
                <FlatList
                    data={filteredEmployees}
                    keyExtractor={this.keyExtractor}
                    renderItem={this.renderItem} />
                </View> 
            );
        } else {
            const { 
                layout,
                innerLayout, 
                avatarContainer, 
                avatarOuterFrame, 
                avatarImage, 
                info,
                name,
                baseText,
                depabbText,
                neighborAvatarContainer, 
                neighborAvatarImage } = styles;

            const layoutFlattenStyle = StyleSheet.flatten([
                layout,
                {
                    width: Dimensions.get('window').width
                }
            ]);

            const neighborTop = ((StyleSheet.flatten(layout).height as number) - (StyleSheet.flatten(neighborAvatarContainer).height as number)) * 0.5;
            const leftNeighborX = - (StyleSheet.flatten(neighborAvatarContainer).height as number) * 0.5;
            const rightNeighborX = Dimensions.get('window').width - (StyleSheet.flatten(neighborAvatarContainer).height as number) * 0.5;
            const opacityValue = neighboursAvatarsVisibility ? fadeInAnim : fadeOutAnim;

            const leftNeighborFlattenStyle = StyleSheet.flatten([
                neighborAvatarContainer,
                {
                    top: neighborTop,
                    left: leftNeighborX
                }]);
            const rightNeighborFlattenStyle = StyleSheet.flatten([
                neighborAvatarContainer,
                {
                    top: neighborTop,
                    left: rightNeighborX
                }]);

            return (
                <Animated.View style={layoutFlattenStyle}>
                    <Animated.View style={{...leftNeighborFlattenStyle, opacity: opacityValue}}>
                        { this.props.leftNeighbor ? <Avatar photo={this.props.leftNeighbor.photo} /> : null }
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
                    <Animated.View style={{...rightNeighborFlattenStyle, opacity: opacityValue}}>
                        { this.props.rightNeighbor ? <Avatar photo={this.props.rightNeighbor.photo} /> : null }
                    </Animated.View>
                </Animated.View>
            );
            }
    }

    private keyExtractor = (item: Employee) => item.employeeId;

    private renderItem = (itemInfo: ListRenderItemInfo<Employee>) => {
        const { item } = itemInfo;
        const { innerLayout, avatarContainer, avatarOuterFrame, avatarImage, info, name, baseText } = tinyStyles;
        return (
            <View style={innerLayout} key={item.employeeId}>
                <View style={avatarContainer}>
                    <Avatar photo={item.photo} style={avatarOuterFrame} imageStyle={avatarImage} />
                </View>
                <View style={info}>
                    <StyledText style={name}>{item.name}, </StyledText>
                    <StyledText style={baseText}>{item.position}</StyledText>
                </View>
            </View>
        );
    }
}
