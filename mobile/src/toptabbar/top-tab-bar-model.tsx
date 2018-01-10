import { TabNavigatorConfig, NavigationRouteConfigMap } from 'react-navigation';
import React from 'react';
import { HelpdeskScreen } from '../helpdesk/helpdesk-screen';
import { TabNavigationOptionsFactory } from '../tabbar/tab-navigation-options-factory';

const navOptionsFactory = new TabNavigationOptionsFactory();

const topTabBarModel: NavigationRouteConfigMap = {

    Department: {
        screen: HelpdeskScreen,
        path: '/helpdesk',
        navigationOptions: navOptionsFactory.create('Department', null, null)
    },
    Room: {
        screen: HelpdeskScreen,
        path: '/helpdesk',
        navigationOptions: navOptionsFactory.create('Room', null, null)
    },
    Company: {
        screen: HelpdeskScreen,
        path: '/helpdesk',
        navigationOptions: navOptionsFactory.create('Company', null, null)
    }
};

export default topTabBarModel;
