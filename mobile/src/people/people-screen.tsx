import React from 'react';
import { StackNavigator, NavigationRouteConfigMap } from 'react-navigation';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { PeopleScreenNavigator } from './navigator/people-screen-navigator';
import { PeopleCompanyFiltered } from './people-company-filtered';
import { SearchView, SearchType } from '../navigation/search-view';

const routeConfig: NavigationRouteConfigMap = {
    PeopleHomeScreen: {
        screen: PeopleScreenNavigator,
        path: '/',
        navigationOptions: {
            header: <SearchView type={SearchType.People}/>,
        }
    },
    CurrentProfile: {
        screen: EmployeeDetailsScreen,
        path: '/profile',
    }
};

export const PeopleScreen = StackNavigator(routeConfig, stackNavigatorConfig);
