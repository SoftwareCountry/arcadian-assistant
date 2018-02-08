import React from 'react';

import { StackNavigator } from 'react-navigation';
import { HomeProfileScreen } from './home-profile-screen';

export const ProfileScreen = StackNavigator({
    Home: {
        screen: HomeProfileScreen,
        path: '/',
    }
});