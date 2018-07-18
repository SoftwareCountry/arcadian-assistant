import React from 'react';
import { StackNavigator, NavigationRouteConfigMap } from 'react-navigation';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { PeopleScreenNavigator } from './navigator/people-screen-navigator';
import { PeopleDepartment } from './people-department';
import { SearchViewPeople } from '../navigation/search-view';

const routeConfig: NavigationRouteConfigMap = {
    PeopleHomeScreen: {
        screen: PeopleScreenNavigator,
        path: '/',
        navigationOptions: {
            header: <SearchViewPeople/>,
        }
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

export const PeopleScreen = StackNavigator(routeConfig, stackNavigatorConfig);
