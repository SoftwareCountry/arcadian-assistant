import { NavigationRouteConfigMap, TabNavigator, TabNavigatorConfig } from 'react-navigation';
import React from 'react';
import peopleScreenNavigatorStyles from './styles';
import { PeopleCompanyFiltered, PeopleDepartmentFiltered, PeopleRoomFiltered } from '../people-filtered';
import { TabBarTopCustom } from './tab-bar-top-custom.component';

//============================================================================
enum PeopleScreenNavigationRouteName {
    department = 'PeopleScreenNavigationRouteName.department',
    room = 'PeopleScreenNavigationRouteName.room',
    company = 'PeopleScreenNavigationRouteName.company',
}

//----------------------------------------------------------------------------
const peopleScreenNavigatorConfig: TabNavigatorConfig = {
    tabBarPosition: 'top',
    tabBarComponent: TabBarTopCustom,
    swipeEnabled: false,
    animationEnabled: false,
    lazy: true,
    tabBarOptions: {
        style: peopleScreenNavigatorStyles.tabBar,
        indicatorStyle: peopleScreenNavigatorStyles.tabBarIndicator,
        upperCaseLabel: false,
        labelStyle: peopleScreenNavigatorStyles.tabBarLabel,
        showIcon: false,
    },
};

//----------------------------------------------------------------------------
const peopleScreenNavigatorModel: NavigationRouteConfigMap = {
    [PeopleScreenNavigationRouteName.department]: {
        screen: PeopleDepartmentFiltered,
        path: '/people/department',
    },
    [PeopleScreenNavigationRouteName.room]: {
        screen: PeopleRoomFiltered,
        path: '/people/room',
    },
    [PeopleScreenNavigationRouteName.company]: {
        screen: PeopleCompanyFiltered,
        path: '/people/company',
    }
};

//----------------------------------------------------------------------------
export const PeopleScreenNavigator = TabNavigator(
    peopleScreenNavigatorModel,
    peopleScreenNavigatorConfig
);
