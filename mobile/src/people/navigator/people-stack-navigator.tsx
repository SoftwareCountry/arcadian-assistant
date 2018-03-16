import React from 'react';
import { StackNavigator, NavigationRouteConfigMap } from 'react-navigation';
import { EmployeeDetailsScreen } from '../../employee-details/employee-details-screen';
import { PeopleHomeScreen } from '../people-screen';
import { stackNavigatorConfig } from '../../override/stack-navigator-config';

const routeConfig: NavigationRouteConfigMap = {
    PeopleHomeScreen: {
        screen: PeopleHomeScreen,
        path: '/',
        navigationOptions: {
            header: null,
        }
    },
    CurrentProfile: {
        screen: EmployeeDetailsScreen,
        path: '/profile',
    }
};

export const PeopleScreen = StackNavigator(routeConfig, stackNavigatorConfig);
