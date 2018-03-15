import React from 'react';
import { StackNavigator } from 'react-navigation';
import { EmployeeDetailsScreen } from '../../employee-details/employee-details-screen';
import { PeopleHomeScreen } from '../people-screen';
import { stackNavigatorConfig } from '../../override/stack-navigator-config';

export const PeopleScreen = StackNavigator({
    PeopleHomeScreen: {
        screen: PeopleHomeScreen,
        path: '/',
    },
    CurrentProfile: {
        screen: EmployeeDetailsScreen,
        path: '/profile',
    }
}, stackNavigatorConfig);


