import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image } from 'react-native';

import { Avatar } from '../people/avatar';
import { Employee } from '../reducers/organization/employee.model';
import { employeesListItemStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';

interface EmployeesListItemProps {
    employee: Employee;
    id: string;
}

export class EmployeesListItem extends React.Component<EmployeesListItemProps> {
    public render() {
        const employeeName = this.props.employee ? this.props.employee.name : 'Unknown';
        const employeePosition = this.props.employee ? this.props.employee.position : 'Unknown';
        const photo = this.props.employee ? this.props.employee.photo : null;

        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <View style={styles.avatarContainer}>
                        <Avatar photo={photo} style={StyleSheet.flatten(styles.avatar)} />
                    </View>
                    <View style={styles.info}>
                        <StyledText style={styles.name}>{employeeName}</StyledText>
                        <StyledText style={styles.baseText}>{employeePosition}</StyledText>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}