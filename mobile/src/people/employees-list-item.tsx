import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image } from 'react-native';

import { Avatar } from '../people/avatar';
import { Employee } from '../reducers/organization/employee.model';
import { employeesListItemStyles as styles } from './styles';

interface EmployeesListItemProps {
    employee: Employee;
    id: string;
}

export class EmployeesListItem extends React.Component<EmployeesListItemProps> {
    public render() {
        const employeeName = this.props.employee ? this.props.employee.name : 'Unknown';
        const employeePosition = this.props.employee ? this.props.employee.position : 'Unknown';
        const photo = this.props.employee ? this.props.employee.photo : null;
        const mimeType = photo ? photo.mimeType : null;
        const base64 = photo ? photo.base64 : null;

        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <View style={styles.imgContainer}>
                        <Avatar mimeType={mimeType} photoBase64={base64} style={styles.img}/>
                    </View>
                    <View style={styles.info}>
                        <Text style={styles.name}>{employeeName}</Text>
                        <Text style={styles.baseText}>{employeePosition}</Text>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}