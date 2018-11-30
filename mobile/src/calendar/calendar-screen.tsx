import { StackNavigator } from 'react-navigation';
import React from 'react';
import { CalendarScreenComponent } from './home-calendar-screen';

//----------------------------------------------------------------------------
export const CalendarScreen = StackNavigator({
    Home: {
        screen: CalendarScreenComponent,
        path: '/',
    },
});
