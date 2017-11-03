import React from 'react';
import { StackNavigator } from 'react-navigation';
import { Text } from 'react-native';

const HomeNavigator = StackNavigator( {
    Sicklist: () => <Text>sicklist</Text>,
    Vacations: () => <Text>vacactions</Text>,
    Actions: () => <Text>actions</Text>
});
