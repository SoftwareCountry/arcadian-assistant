import { createStackNavigator } from 'react-navigation';
import React from 'react';
import { CalendarScreenComponent } from './home-calendar-screen';
import { transparentHeaderStackNavigatorConfig } from '../override/stack-navigator-config';

//----------------------------------------------------------------------------
export const CalendarScreen = createStackNavigator({
    Home: {
        screen: CalendarScreenComponent,
        path: '/',
    },
},                                                 transparentHeaderStackNavigatorConfig);
