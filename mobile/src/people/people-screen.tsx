import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';

export class PeopleScreen extends Component {

    public render() {
        return <View style={styles.container}>
            <Text>People</Text>
        </View>;
    }
}