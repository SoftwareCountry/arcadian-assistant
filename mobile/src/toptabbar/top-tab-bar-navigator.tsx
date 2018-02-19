import { TabNavigator, TabNavigatorConfig, TabBarTop } from 'react-navigation';
import React from 'react';
import topTabBarStyles from './top-tab-bar-styles';
import peopleTopTabBarModel from './people-top-tab-bar-model';

const topTabNavigatorConfig: TabNavigatorConfig = {
    tabBarPosition: 'top',
    tabBarComponent: TabBarTop, 
    swipeEnabled: false,
    animationEnabled: false,
    lazy: true,
    tabBarOptions: {
        style: topTabBarStyles.tabBar,
        indicatorStyle: topTabBarStyles.tabBarIndicator,
        upperCaseLabel: false,
        labelStyle: topTabBarStyles.tabBarLabel,
        showIcon: false,
    },
};

export const  TopTabBarNavigator = TabNavigator(
    peopleTopTabBarModel,
    topTabNavigatorConfig
);
