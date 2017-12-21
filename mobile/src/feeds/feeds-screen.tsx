import { StackNavigator } from 'react-navigation';
import React from 'react';
import { HomeFeedsScreen } from './home-feeds-screen';
import { SicklistScreen } from '../home/sicklist-screen';
import { HelpdeskScreen } from '../home/helpdesk/helpdesk-screen';
import { DaysoffScreen } from '../home/daysoff-screen';
import { VacationsScreen } from '../home/vacations-screen';

export const FeedsScreen = StackNavigator({
    Home: {
        screen: HomeFeedsScreen,
        path: '/',
    }
});