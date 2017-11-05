import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';

export class ManagementScreen extends Component {

    public render() {
        return <View style={styles.container}>
            <Text>Management</Text>
        </View>;
    }
}