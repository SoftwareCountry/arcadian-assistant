import React, { Component } from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { View, Text } from 'react-native';
import { TabNavigationOptionsFactory } from '../layout/tab-navigation-options-factory';

const navOptionsFactory = new TabNavigationOptionsFactory();

export class ManagementScreen extends Component {

    static navigationOptions: NavigationTabScreenOptions = 
        navOptionsFactory.create('Management', 'ios-laptop', 'ios-laptop-outline');

    render() {
        return <View><Text>Management</Text></View>;
    }
}