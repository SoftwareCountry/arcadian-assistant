import { TabNavigatorConfig, NavigationRouteConfigMap } from 'react-navigation';
import React from 'react';
import { EmployeesList } from '../people/employees-list';
import { TabNavigationOptionsFactory } from '../tabbar/tab-navigation-options-factory';

const navOptionsFactory = new TabNavigationOptionsFactory();

const topTabBarModel: NavigationRouteConfigMap = {

    Department: {
        screen: EmployeesList,
        path: '/people',
        navigationOptions: navOptionsFactory.create('Department', null, null)
    },
    Room: {
        screen: EmployeesList,
        path: '/people',
        navigationOptions: navOptionsFactory.create('Room', null, null)
    },
    Company: {
        screen: EmployeesList,
        path: '/people',
        navigationOptions: navOptionsFactory.create('Company', null, null)
    }
};

export default topTabBarModel;
