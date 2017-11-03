import { TabNavigator } from 'react-navigation';
import React from 'react';
import { View, Text } from 'react-native';
import { ManagementScreen } from '../management/management-screen';
import { HomeScreen } from '../home/home-screen';
import { FeedScreen } from '../feed/feed-screen';
import { OrganizationScreen } from '../organization/organization-screen';

export const RootNavigator = TabNavigator({
    Home: {
        screen: HomeScreen
    },
    Feed: {
        screen: FeedScreen
    },
    Organization: {
        screen: OrganizationScreen
    },
    Management: {
        screen: ManagementScreen
    }
})