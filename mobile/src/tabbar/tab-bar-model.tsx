import { TabNavigatorConfig, NavigationRouteConfigMap } from 'react-navigation';
import React from 'react';
import { FeedsScreen } from '../feeds/feeds-screen';
import { HelpdeskScreen } from '../helpdesk/helpdesk-screen';
import { CalendarScreen } from '../calendar/calendar-screen';
import { ProfileScreen } from '../profile/profile-screen';
import { TabNavigationOptionsFactory } from '../tabbar/tab-navigation-options-factory';
import { PeopleScreen } from '../people/people-screen';
import {calendarTabBarOnPressHandler, feedsTabBarOnPressHandler, peopleTabBarOnPressHandler} from './tab-bar-on-press-handlers';

const navOptionsFactory = new TabNavigationOptionsFactory();

const tabbarModel: NavigationRouteConfigMap = {

    News: {
        screen: FeedsScreen,
        path: '/',
        navigationOptions: navOptionsFactory.create('Feeds', 'feeds', feedsTabBarOnPressHandler)
    },
    People: {
        screen: PeopleScreen,
        path: '/people',
        navigationOptions: navOptionsFactory.create('People', 'people')
    },
    Calendar: {
        screen: CalendarScreen,
        path: '/calendar',
        navigationOptions: navOptionsFactory.create('Calendar', 'calendar', calendarTabBarOnPressHandler)
    },
    Profile: {
        screen: ProfileScreen,
        path: '/profile',
        navigationOptions: navOptionsFactory.create('Profile', 'profile')
    }
};

export default tabbarModel;
