import React from 'react';
import { StyleSheet, Platform, Text, View, Image, TouchableOpacity } from 'react-native';
import { Avatar } from '../people/avatar';
import { Employee } from '../reducers/organization/employee.model';
import { employeesListItemStyles as styles } from './styles';
import { StyledText } from '../override/styled-text';
import { Dispatch, connect } from 'react-redux';
import { mapEmployeeDetailsDispatchToProps, EmployeeDetailsProps } from '../layout/user-selected-dispatcher';

interface EmployeesListItemProps {
    employee: Employee;
    id: string;
}

const mapDispatchToProps = (dispatch: Dispatch<any>): EmployeeDetailsProps => ({
    ...mapEmployeeDetailsDispatchToProps(dispatch)
});

export class EmployeesListItemImpl extends React.Component<EmployeesListItemProps & EmployeeDetailsProps> {
    public render() {
        const employeeName = this.props.employee ? this.props.employee.name : null;
        const employeePosition = this.props.employee ? this.props.employee.position : null;
        const photo = this.props.employee ? this.props.employee.photo : null;

        return (
            <TouchableOpacity onPress = {() => this.props.onAvatarClicked(this.props.employee)}>
                <View style={styles.layout}>
                    <View style={styles.avatarContainer}>                
                        <Avatar photo={photo} style={StyleSheet.flatten(styles.avatarOuterFrame)} imageStyle={StyleSheet.flatten(styles.avatarImage)} />
                    </View>
                    <View style={styles.info}>
                        <StyledText style={styles.name}>{employeeName}</StyledText>
                        <StyledText style={styles.baseText}>{employeePosition}</StyledText>
                    </View>
                </View>
             </TouchableOpacity>
        );
    }
}

export const EmployeesListItem = connect(null, mapDispatchToProps)(EmployeesListItemImpl);
