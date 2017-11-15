import { StackNavigator } from 'react-navigation';
import React from 'react';
import { HomeScreen } from '../home/home-screen';
import { SicklistScreen } from '../home/sicklist-screen';
import { HelpdeskScreen } from '../home/helpdesk-screen';
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
    Helpdesk: {
        screen: HelpdeskScreen,
        path: '/helpdesk',
        navigationOptions: {
            title: 'Helpdesk'
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