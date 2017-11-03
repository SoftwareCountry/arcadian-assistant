import React, { Component } from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { View, Text } from 'react-native';
import { TabNavigationOptionsFactory } from '../layout/tab-navigation-options-factory';

const navOptionsFactory = new TabNavigationOptionsFactory();

export class OrganizationScreen extends Component {

    static navigationOptions: NavigationTabScreenOptions = 
        navOptionsFactory.create('Organization', 'ios-people', 'ios-people-outline');;

    render() {
        return <View><Text>Organization</Text></View>;
    }
}