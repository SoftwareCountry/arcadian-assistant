import { TabNavigator, TabNavigatorConfig, TabBarTop } from 'react-navigation';
import React from 'react';
import topTabBarStyles from './top-tab-bar-styles';
import topTabBarModel from './top-tab-bar-model';

const topTabNavigatorConfig: TabNavigatorConfig = {
    tabBarPosition: 'top',
    tabBarComponent: TabBarTop, 
    tabBarOptions: {
        style: topTabBarStyles.tabBar,
        indicatorStyle: topTabBarStyles.tabBarIndicator,
        upperCaseLabel: false,
        labelStyle: topTabBarStyles.tabBarLabel,
        showIcon: false,
    },
    animationEnabled: true,
};

export const  TopTabBarNavigator = TabNavigator(
    topTabBarModel,
    topTabNavigatorConfig,
);
