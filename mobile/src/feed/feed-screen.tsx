import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';

export class FeedScreen extends Component {
    public render() {
        return <View style={styles.container}>
            <Text>Feed</Text>
        </View>;
    }
}