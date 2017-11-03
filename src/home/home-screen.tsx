import React, { Component } from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { View, Text } from 'react-native';
import Icon from 'react-native-vector-icons/Ionicons';

const navigationOptions: NavigationTabScreenOptions =
{
    tabBarLabel: 'Home',
    tabBarIcon: ({ tintColor, focused }) =>
        <Icon
            name={focused ? 'ios-home' : 'ios-home-outline'}
            size={20}
            style={{ color: tintColor }}
        >
        </Icon>
}

export class HomeScreen extends Component {

    static navigationOptions: NavigationTabScreenOptions = navigationOptions;

    render() {
        return <View><Text>Test</Text></View>;
    }
}