import React, { Component } from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { View, Text } from 'react-native';
import { Ionicons } from 'react-native-vector-icons';

const navigationOptions: NavigationTabScreenOptions =
{
    tabBarLabel: 'Feeds',
    tabBarIcon: ({ tintColor, focused }) =>
        <Ionicons
            name={focused ? 'ios-pulse' : 'ios-pulse-outline'}
            size={20}
            style={{ color: tintColor }}
        >
        </Ionicons>
}

export class FeedScreen extends Component {

    static navigationOptions: NavigationTabScreenOptions = navigationOptions;

    render() {
        return <View><Text>Feed</Text></View>;
    }
}