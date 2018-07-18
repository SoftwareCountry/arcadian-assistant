import React, { Component } from 'react';
import { View, Image, Dimensions, StyleSheet, FlatList, ListRenderItemInfo, TouchableOpacity } from 'react-native';
import { Avatar } from '../avatar';
import { Employee } from '../../reducers/organization/employee.model';
import { StyledText } from '../../override/styled-text';
import { companyItemStyles as styles, companyTinyItemStyles as tinyStyles} from '../styles';

interface EmployeeCardWithAvatarProps {
    chiefId: string;
    employees: Employee[];
    treeLevel: number;
    onItemClicked: (e: Employee) => void;
}

export class EmployeeCardWithAvatarList extends Component<EmployeeCardWithAvatarProps> {
    public render() {
        const calcultatedHeight = Dimensions.get('screen').height - 90 * this.props.treeLevel - 119;
        const { layout } = styles;
        const layoutFlattenStyle = StyleSheet.flatten([
            layout, {width: Dimensions.get('window').width, height: calcultatedHeight}
        ]);
        
        return (
            <View style={layoutFlattenStyle}>
                <FlatList
                    data={this.props.employees}
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
            <TouchableOpacity onPress={() => this.onItemClicked(itemInfo.index)}>
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

    private onItemClicked = (itemId: number) => {
        const filteredEmployees = this.props.employees.filter((emp) => emp.employeeId !== this.props.chiefId);
        this.props.onItemClicked(filteredEmployees[itemId]);
    }
}
