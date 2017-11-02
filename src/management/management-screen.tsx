import React, { Component } from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { View, Text } from 'react-native';
import { Ionicons } from 'react-native-vector-icons';

const navigationOptions: NavigationTabScreenOptions =
{
    tabBarLabel: 'Management',
    tabBarIcon: ({ tintColor, focused }) =>
        <Ionicons
            name={focused ? 'ios-laptop' : 'ios-laptop-outline'}
            size={20}
            style={{ color: tintColor }}
        >
        </Ionicons>
}

export class ManagementScreen extends Component {

    static navigationOptions: NavigationTabScreenOptions = navigationOptions;

    render() {
        return <View><Text>Management</Text></View>;
    }
}