import React from 'react';
import { StackNavigator, NavigationRouteConfigMap } from 'react-navigation';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { PeopleScreenNavigator } from './navigator/people-screen-navigator';
import { SearchViewPeople } from '../navigation/search-view';
import { CurrentPeopleDepartment } from './current-people-department';
import { CurrentPeopleRoom } from './current-people-room';

const routeConfig: NavigationRouteConfigMap = {
    PeopleHomeScreen: {
        screen: PeopleScreenNavigator,
        path: '/',
        navigationOptions:  {
            headerTitle: <SearchViewPeople/>,
        }
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

export const PeopleScreen = StackNavigator(routeConfig, stackNavigatorConfig);
