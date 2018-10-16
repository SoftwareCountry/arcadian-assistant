import React from 'react';

import { StackNavigator, NavigationRouteConfigMap } from 'react-navigation';
import { HomeFeedsScreen } from './home-feeds-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';
import { CurrentPeopleDepartment } from '../people/current-people-department';
import { CurrentPeopleRoom } from '../people/current-people-room';
import { CompanyDepartments } from '../people/company-departments';

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
        screen: CurrentPeopleDepartment,
        path: '/current-department'
    },
    CurrentRoom: {
        screen: CurrentPeopleRoom,
        path: '/current-room'
    }
};

export const FeedsScreen = StackNavigator(routeConfig, stackNavigatorConfig);