import React, { Component } from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { View, Text } from 'react-native';
import Icon from 'react-native-vector-icons/Ionicons';
import { TabNavigationOptionsFactory } from '../layout/tab-navigation-options-factory';

const navOptionsFactory = new TabNavigationOptionsFactory();

export class HomeScreen extends Component {

    static navigationOptions: NavigationTabScreenOptions = 
        navOptionsFactory.create('Feeds', 'ios-home', 'ios-home-outline');

    render() {
        return <View><Text>Test</Text></View>;
    }
}