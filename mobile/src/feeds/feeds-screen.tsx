import React from 'react';

import { StackNavigator } from 'react-navigation';
import { HomeFeedsScreen } from './home-feeds-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';

export const FeedsScreen = StackNavigator({
    Home: {
        screen: HomeFeedsScreen,
        path: '/',
    },
    CurrentProfile: {
        screen: EmployeeDetailsScreen,
        path: '/profile'
    }
}, stackNavigatorConfig);