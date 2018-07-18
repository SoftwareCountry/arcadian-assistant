import React from 'react';

import { StackNavigator, NavigationRouteConfigMap } from 'react-navigation';
import { HomeFeedsScreen } from './home-feeds-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';
import { PeopleDepartment } from '../people/people-department';

const routeConfig: NavigationRouteConfigMap = {
    PeopleHomeScreen: {
        screen: HomeFeedsScreen,
        path: '/',
    },
    CurrentProfile: {
        screen: EmployeeDetailsScreen,
        path: '/profile',
    },
    CurrentDepartment: {
        screen: PeopleDepartment,
        path: '/current-department'
    }
};

export const FeedsScreen = StackNavigator(routeConfig, stackNavigatorConfig);