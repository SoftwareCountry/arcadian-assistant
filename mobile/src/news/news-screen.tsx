import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';

export class NewsScreen extends Component {
    public render() {
        return <View style={styles.container}>
            <Text>News</Text>
        </View>;
    }
}