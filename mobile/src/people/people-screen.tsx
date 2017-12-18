import { StackNavigator } from 'react-navigation';
import React from 'react';
import { HomePeopleScreen } from './home-people-screen';

export const PeopleScreen = StackNavigator({
    Home: {
        screen: HomePeopleScreen,
        path: '/',
    },
});