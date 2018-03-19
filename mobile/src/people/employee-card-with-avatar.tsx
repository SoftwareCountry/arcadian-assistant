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
              duration: 3000,
            }
          ).start();
  
        Animated.timing(
              this.state.fadeOutAnim,
              {
                toValue: 0,
                duration: 2000,
              }
          ).start();
  
        const employees = this.props.employees;
        const calcultatedHeight = Dimensions.get('screen').height - 90 - 194;
        console.log('Calculated height: ' + calcultatedHeight);
        if (employees != null) {
            const filteredEmployees = employees.filter((emp) => emp.employeeId !== this.props.chiefId);
            return (
                <View style={{ backgroundColor: '#abfcba', width: Dimensions.get('window').width, height: calcultatedHeight, overflow: 'hidden', borderTopWidth: 1, borderColor: 'rgba(0, 0, 0, 0.15)' }}>
                <FlatList
                    data={filteredEmployees}
                    keyExtractor={this.keyExtractor}
                    renderItem={this.renderItem} />
                </View> 
            );
        } else {
            return (
                <Animated.View style={{ width: Dimensions.get('window').width, height: 90, overflow: 'hidden', borderTopWidth: 1, borderColor: 'rgba(0, 0, 0, 0.15)' }}>
                    <Animated.View style={{ position: 'absolute', top: (90 - 44) * 0.5, left: -44 * 0.5, opacity: neighboursAvatarsVisibility ? fadeInAnim : fadeOutAnim }}>
                        { this.props.leftNeighbor ? <Avatar photo={this.props.leftNeighbor.photo} style={{ width: 44, height: 44 }} /> : null }
                    </Animated.View>
                    <View style={styles.layout}>
                        <View style={styles.avatarContainer}>
                            <Avatar photo={photo} style={{ width: 56, height: 56 }} />
                        </View>
                        <View style={styles.info}>
                            <StyledText style={styles.name}>{employee.name}</StyledText>
                            <StyledText style={styles.baseText}>{employee.position}</StyledText>
                            <StyledText style={styles.depabbText}>{this.props.departmentAbbreviation}</StyledText>
                        </View>
                    </View>
                    <Animated.View style={{ position: 'absolute', top: (90 - 44) * 0.5, left: (Dimensions.get('window').width - 44 * 0.5 ), opacity: neighboursAvatarsVisibility ? fadeInAnim : fadeOutAnim}}>
                        { this.props.rightNeighbor ? <Avatar photo={this.props.rightNeighbor.photo} style={{ width: 44, height: 44 }} /> : null }
                    </Animated.View>
                </Animated.View>
            );
            }
    }

    private keyExtractor = (item: Employee) => item.employeeId;

    private renderItem = (itemInfo: ListRenderItemInfo<Employee>) => {
        const { item } = itemInfo;

        return (
            <View style={tinyStyles.layout} key={item.employeeId}>
                <View style={tinyStyles.avatarContainer}>
                    <Avatar photo={item.photo} style={tinyStyles.avatarOuterFrame} imageStyle={tinyStyles.avatarImage} />
                </View>
                <View style={tinyStyles.info}>
                    <StyledText style={tinyStyles.name}>{item.name}, </StyledText>
                    <StyledText style={tinyStyles.baseText}>{item.position}</StyledText>
                </View>
            </View>
        );
    }
}
