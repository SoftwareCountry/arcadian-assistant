import { TabNavigatorConfig, NavigationRouteConfigMap } from 'react-navigation';
import React from 'react';
import { FeedsScreen } from '../feeds/feeds-screen';
import { PeopleScreen } from '../people/navigator/people-stack-navigator';
import { HelpdeskScreen } from '../helpdesk/helpdesk-screen';
import { CalendarScreen } from '../calendar/calendar-screen';
import { ProfileScreen } from '../profile/profile-screen';
import { TabNavigationOptionsFactory } from '../tabbar/tab-navigation-options-factory';

const navOptionsFactory = new TabNavigationOptionsFactory();

const tabbarModel: NavigationRouteConfigMap = {

    News: {
        screen: FeedsScreen,
        path: '/',
        navigationOptions: navOptionsFactory.create('Feeds', 'feeds')
    },
    People: {
        screen: PeopleScreen,
        path: '/people',
        navigationOptions: navOptionsFactory.create('People', 'people')
    },
    Calendar: {
        screen: CalendarScreen,
        path: '/calendar',
        navigationOptions: navOptionsFactory.create('Calendar', 'calendar')
    },
    Profile: {
        screen: ProfileScreen,
        path: '/profile',
        navigationOptions: navOptionsFactory.create('Profile', 'profile')
    }
};

export default tabbarModel; 
