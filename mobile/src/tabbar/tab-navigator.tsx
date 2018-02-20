import { TabNavigator, TabNavigatorConfig } from 'react-navigation';
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
<<<<<<< HEAD
    animationEnabled: false,
    lazy: true
=======
    animationEnabled: false
>>>>>>> 30bb38a237a06b225a4e2ded8f6d85dddf5defe9
};

export const RootNavigator = TabNavigator(
    tabbarModel,
    tabNavigatorConfig,
);
