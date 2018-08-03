import React from 'react';
import { StyleSheet, Platform, Text, View, Image, TouchableOpacity } from 'react-native';
import { Avatar } from '../people/avatar/avatar';
import { Employee } from '../reducers/organization/employee.model';
import { employeesListItemStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';

interface EmployeesListItemProps {
    employee: Employee;
    id: string;
    onItemClicked: (e: Employee) => void;
}

export class EmployeesListItem extends React.Component<EmployeesListItemProps> {
    public render() {
        const employeeName = this.props.employee ? this.props.employee.name : null;
        const employeePosition = this.props.employee ? this.props.employee.position : null;
        const avatar = this.props.employee ?  
            <Avatar id={this.props.employee.employeeId} style={StyleSheet.flatten(styles.avatarOuterFrame)} 
                imageStyle={StyleSheet.flatten(styles.avatarImage)} /> 
            : null;

        return (
            <TouchableOpacity onPress = {this.onItemClicked}>
                <View style={styles.layout}>
                    <View style={styles.avatarContainer}>                
                        {avatar}
                    </View>
                    <View style={styles.info}>
                        <StyledText style={styles.name}>{employeeName}</StyledText>
                        <StyledText style={styles.baseText}>{employeePosition}</StyledText>
                    </View>
                </View>
             </TouchableOpacity>
        );
    }

    private onItemClicked = () => this.props.onItemClicked(this.props.employee);

}
