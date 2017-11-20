import { TabNavigator } from 'react-navigation';
import React from 'react';
import { View, Text } from 'react-native';
import { ManagementScreen } from '../management/management-screen';
import { HomeScreen } from '../home/home-screen';
import { FeedScreen } from '../feed/feed-screen';
import { OrganizationScreen } from '../organization/organization-screen';
import { TabNavigationOptionsFactory } from './tab-navigation-options-factory';
import { MainTabNavigator } from './main-tab-navigator';

const navOptionsFactory = new TabNavigationOptionsFactory();

export const RootNavigator = TabNavigator({
    MainTab: {
        screen: MainTabNavigator,
        path: '/',
        navigationOptions: navOptionsFactory.create('Home', 'ios-home', 'ios-home-outline')
    },
    Feed: {
        screen: FeedScreen,
        path: '/feeds',
        navigationOptions: navOptionsFactory.create('Feeds', 'ios-pulse', 'ios-pulse-outline')
    },
    Organization: {
        screen: OrganizationScreen,
        path: '/organization',
        navigationOptions: navOptionsFactory.create('Organization', 'ios-people', 'ios-people-outline')
    },
    Management: {
        screen: ManagementScreen,
        path: '/management',
        navigationOptions: navOptionsFactory.create('Management', 'ios-laptop', 'ios-laptop-outline')
    }
});