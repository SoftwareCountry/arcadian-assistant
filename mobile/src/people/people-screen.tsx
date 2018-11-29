import React from 'react';
import { NavigationRouteConfigMap, createStackNavigator } from 'react-navigation';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { PeopleScreenNavigator } from './navigator/people-screen-navigator';
import { SearchViewPeople } from '../navigation/search-view';
import { CurrentPeopleDepartment } from './current-people-department';
import { CurrentPeopleRoom } from './current-people-room';
import Style from '../layout/style';

const routeConfig: NavigationRouteConfigMap = {
    PeopleHomeScreen: {
        screen: PeopleScreenNavigator,
        path: '/',
        navigationOptions:  {
            headerTitle: <SearchViewPeople/>,
            headerStyle: {
                backgroundColor: Style.color.base
            },
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

export const PeopleScreen = createStackNavigator(routeConfig, stackNavigatorConfig);
