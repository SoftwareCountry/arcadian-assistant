import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';

export class ProfileScreen extends Component {

    public render() {
        return <View style={styles.container}>
            <Text>Profile</Text>
        </View>;
    }
}