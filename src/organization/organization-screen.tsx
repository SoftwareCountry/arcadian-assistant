import React, { Component } from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { View, Text } from 'react-native';
import { Ionicons } from 'react-native-vector-icons';

const navigationOptions: NavigationTabScreenOptions =
{
    tabBarLabel: 'Organization',
    tabBarIcon: ({ tintColor, focused }) =>
        <Ionicons
            name={focused ? 'ios-people' : 'ios-people-outline'}
            size={20}
            style={{ color: tintColor }}
        >
        </Ionicons>
}

export class OrganizationScreen extends Component {

    static navigationOptions: NavigationTabScreenOptions = navigationOptions;

    render() {
        return <View><Text>Organization</Text></View>;
    }
}