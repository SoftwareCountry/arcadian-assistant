import { TabNavigatorConfig, TabBarTop, TabNavigator } from 'react-navigation';
import React from 'react';
import peopleScreenNavigatorStyles from './styles';
import peopleScreenNavigatorModel from './people-screen-navigator-model';
import { RootNavigator } from '../../tabbar/tab-navigator';

const peopleScreenNavigatorConfig: TabNavigatorConfig = {
    tabBarPosition: 'top',
    tabBarComponent: TabBarTop, 
    swipeEnabled: false,
    animationEnabled: false,
    lazy: true,
    tabBarOptions: {
        style: peopleScreenNavigatorStyles.tabBar,
        indicatorStyle: peopleScreenNavigatorStyles.tabBarIndicator,
        upperCaseLabel: false,
        labelStyle: peopleScreenNavigatorStyles.tabBarLabel,
        showIcon: false,
    },
};

export const  PeopleScreenNavigator = TabNavigator(
    peopleScreenNavigatorModel,
    peopleScreenNavigatorConfig
);