import { StackNavigator } from 'react-navigation';
import React from 'react';
import { CalendarScreenImpl } from './home-calendar-screen';

export const CalendarScreen = StackNavigator({
    Home: {
        screen: CalendarScreenImpl,
        path: '/',
    },
});