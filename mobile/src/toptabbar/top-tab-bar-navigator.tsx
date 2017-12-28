import { TabNavigator, TabNavigatorConfig } from 'react-navigation';
import React from 'react';
import topTabBarStyles from './top-tab-bar-styles';
import topTabBarModel from './top-tab-bar-model';

const topTabNavigatorConfig: TabNavigatorConfig = {
    tabBarPosition: 'top',
    tabBarOptions: {
        style: topTabBarStyles.tabBar,
        indicatorStyle: topTabBarStyles.tabBarIndicator,
        upperCaseLabel: true,
        labelStyle: topTabBarStyles.tabBarLabel,
        showIcon: false,
    },
    animationEnabled: true,
};

export const  topTabBarNavigator = TabNavigator(
    topTabBarModel,
    topTabNavigatorConfig,
);
