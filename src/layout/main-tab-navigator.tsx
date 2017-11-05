import { StackNavigator } from 'react-navigation';
import React from 'react';
import { HomeScreen } from '../home/home-screen';
import { SicklistScreen } from '../home/sicklist-screen';
import { ActionsScreen } from '../home/actions-screen';
import { DaysoffScreen } from '../home/daysoff-screen';
import { VacationsScreen } from '../home/vacations-screen';

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
    },
    Actions: {
        screen: ActionsScreen,
        path: '/actions',
        navigationOptions: {
            title: 'Actions'
        }
    },
    Daysoff: {
        screen: DaysoffScreen,
        path: '/daysoff',
        navigationOptions: {
            title: 'Daysoff'
        }
    },
    Vacations: {
        screen: VacationsScreen,
        path: '/vacations',
        navigationOptions: {
            title: 'Vacations'
        }
    },
});