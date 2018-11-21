import React from 'react';
import { NavigationRouteConfigMap, StackNavigator } from 'react-navigation';
import { HomeFeedsScreen } from './home-feeds-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';
import { CurrentPeopleDepartment } from '../people/current-people-department';
import { CurrentPeopleRoom } from '../people/current-people-room';

//----------------------------------------------------------------------------
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

//----------------------------------------------------------------------------
export const FeedsScreen = StackNavigator(routeConfig, stackNavigatorConfig);
