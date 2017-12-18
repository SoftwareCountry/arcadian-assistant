import { StackNavigator } from 'react-navigation';
import React from 'react';
import { HomeNewsScreen } from './home-news-screen';
import { SicklistScreen } from '../home/sicklist-screen';
import { HelpdeskScreen } from '../home/helpdesk/helpdesk-screen';
import { DaysoffScreen } from '../home/daysoff-screen';
import { VacationsScreen } from '../home/vacations-screen';

export const NewsScreen = StackNavigator({
    Home: {
        screen: HomeNewsScreen,
        path: '/',
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