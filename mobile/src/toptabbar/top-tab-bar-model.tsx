import { TabNavigatorConfig, NavigationRouteConfigMap } from 'react-navigation';
import React from 'react';
import { PeopleScreen } from '../people/people-screen';
import { TabNavigationOptionsFactory } from '../tabbar/tab-navigation-options-factory';

const navOptionsFactory = new TabNavigationOptionsFactory();

const topTabBarModel: NavigationRouteConfigMap = {

    Department: {
        screen: PeopleScreen,
        path: '/people',
        navigationOptions: navOptionsFactory.create('Department', null, null)
    },
    Room: {
        screen: PeopleScreen,
        path: '/people',
        navigationOptions: navOptionsFactory.create('Room', null, null)
    },
    Company: {
        screen: PeopleScreen,
        path: '/people',
        navigationOptions: navOptionsFactory.create('Company', null, null)
    }
};

export default topTabBarModel;
