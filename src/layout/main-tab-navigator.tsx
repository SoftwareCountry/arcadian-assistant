import { StackNavigator } from 'react-navigation';
import React from 'react';
import { HomeScreen } from '../home/home-screen';
import { SicklistScreen } from '../home/sicklist-screen';

export const MainTabNavigator = StackNavigator({
     Home: {
        screen: HomeScreen,
        path: '/',
        navigationOptions: {
            title: 'Welcome'
        }
    },
    Sicklist: {
        screen: SicklistScreen,
        path: '/sicklist',
        navigationOptions: {
            title: 'Sicklist'
        }
    }
});