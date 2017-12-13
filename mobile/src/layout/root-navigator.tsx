import { TabNavigator, TabNavigatorConfig } from 'react-navigation';
import React from 'react';
import { View, Text, Dimensions } from 'react-native';
import { ManagementScreen } from '../management/management-screen';
import { HomeScreen } from '../home/home-screen';
import { FeedScreen } from '../feed/feed-screen';
import { OrganizationScreen } from '../organization/organization-screen';
import { TabNavigationOptionsFactory } from './tab-navigation-options-factory';
import { MainTabNavigator } from './main-tab-navigator';

const navOptionsFactory = new TabNavigationOptionsFactory();
const tabNavigatorConfig: TabNavigatorConfig = {
    tabBarPosition: 'bottom',
    tabBarOptions: {
        style: {
            height: 59,
            backgroundColor: '#2FAFCC',
        },
        indicatorStyle: {
            backgroundColor: 'transparent',
        },
        upperCaseLabel: false,
        labelStyle: {
            color: '#FFFFFF',
            fontSize: 10,
            lineHeight: 12,
            textAlign: 'center',
            marginBottom: 5,
        },
        showIcon: true,
        tabStyle: {
            width: Dimensions.get('window').width / 5
        },
    },
    animationEnabled: false,
};

export const RootNavigator = TabNavigator(
    {
        MainTab: {
            screen: MainTabNavigator,
            path: '/',
            navigationOptions: navOptionsFactory.create('Home', require('../../src/images/inactive-tab-bar/News.png'), require('../../src/images/inactive-tab-bar/News.png'))
        },
        Feed: {
            screen: FeedScreen,
            path: '/feeds',
            navigationOptions: navOptionsFactory.create('People', require('../../src/images/inactive-tab-bar/People.png'), require('../../src/images/inactive-tab-bar/People.png'))
        },
        Organization: {
            screen: OrganizationScreen,
            path: '/organization',
            navigationOptions: navOptionsFactory.create('Helpdesk', require('../../src/images/inactive-tab-bar/Helpdesk.png'), require('../../src/images/inactive-tab-bar/Helpdesk.png'))
        },
        Management: {
            screen: ManagementScreen,
            path: '/management',
            navigationOptions: navOptionsFactory.create('Calendar', require('../../src/images/inactive-tab-bar/Calendar.png'), require('../../src/images/inactive-tab-bar/Calendar.png'))
        },
        Management2: {
            screen: ManagementScreen,
            path: '/management',
            navigationOptions: navOptionsFactory.create('Profile', require('../../src/images/inactive-tab-bar/Profile.png'), require('../../src/images/inactive-tab-bar/Profile.png'))
        }
    },
    tabNavigatorConfig,
);
