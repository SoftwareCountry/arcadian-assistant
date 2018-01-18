import React from 'react';

import { StackNavigator } from 'react-navigation';
import { HomeFeedsScreen } from './home-feeds-screen';

export const FeedsScreen = StackNavigator({
    Home: {
        screen: HomeFeedsScreen,
        path: '/',
    }
});