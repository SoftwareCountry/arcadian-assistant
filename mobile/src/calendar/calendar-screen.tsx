import { createStackNavigator } from 'react-navigation';
import React from 'react';
import { CalendarScreenComponent } from './home-calendar-screen';

//----------------------------------------------------------------------------
export const CalendarScreen = createStackNavigator({
    Home: {
        screen: CalendarScreenComponent,
        path: '/',
    },
});
