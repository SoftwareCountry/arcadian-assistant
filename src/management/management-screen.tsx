import React, { Component } from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { View, Text } from 'react-native';
import { TabNavigationOptionsFactory } from '../layout/tab-navigation-options-factory';
import styles from '../layout/styles';

const navOptionsFactory = new TabNavigationOptionsFactory();

export class ManagementScreen extends Component {

    static navigationOptions: NavigationTabScreenOptions =
    navOptionsFactory.create('Management', 'ios-laptop', 'ios-laptop-outline');

    render() {
        return <View style={styles.container}>
            <Text>Management</Text>
        </View>;
    }
}