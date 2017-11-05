import React from 'react';
import { StackNavigator } from 'react-navigation';
import { Text } from 'react-native';
import { HomeScreen } from './home-screen';

export const HomeNavigator = StackNavigator( {
    Home: { screen: () => HomeScreen },
    Sicklist: { screen: () => <Text>sicklist</Text> },
    Daysoff: { screen: () => <Text>daysoff</Text> },
    Vacations: { screen: () => <Text>vacations</Text> },
    Actions: { screen: () => <Text>actions</Text> },
});
