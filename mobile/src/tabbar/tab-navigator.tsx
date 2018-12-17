import { createAppContainer, createBottomTabNavigator, TabNavigatorConfig } from 'react-navigation';
import React from 'react';
import tabBarStyles from './tab-bar-styles';
import tabbarModel from './tab-bar-model';

const tabNavigatorConfig: TabNavigatorConfig = {
    tabBarPosition: 'bottom',
    tabBarOptions: {
        style: tabBarStyles.tabBar,
        indicatorStyle: tabBarStyles.tabBarIndicator,
        upperCaseLabel: false,
        labelStyle: tabBarStyles.tabBarLabel,
        showIcon: true,
    },
    animationEnabled: false,
    swipeEnabled: false,
    lazy: true
};

export const RootNavigator = createAppContainer(createBottomTabNavigator(
    tabbarModel,
    tabNavigatorConfig,
));
