import { TabNavigatorConfig, NavigationRouteConfigMap } from 'react-navigation';
import React from 'react';
import { FeedsScreen } from '../feeds/feeds-screen';
import { PeopleScreen } from '../people/people-screen';
import { HelpdeskScreen } from '../helpdesk/helpdesk-screen';
import { CalendarScreen } from '../calendar/calendar-screen';
import { ProfileScreen } from '../profile/profile-screen';
import { TabNavigationOptionsFactory } from '../tabbar/tab-navigation-options-factory';

const navOptionsFactory = new TabNavigationOptionsFactory();

const tabbarModel: NavigationRouteConfigMap = {

    News: {
        screen: FeedsScreen,
        path: '/',
        navigationOptions: navOptionsFactory.create('Feeds', require('../../src/tabbar/News.png'), require('../../src/tabbar/News.png'))
    },
    People: {
        screen: PeopleScreen,
        path: '/people',
        navigationOptions: navOptionsFactory.create('People', require('../../src/tabbar/People.png'), require('../../src/tabbar/People.png'))
    },
    Helpdesk: {
        screen: HelpdeskScreen,
        path: '/helpdesk',
        navigationOptions: navOptionsFactory.create('Helpdesk', require('../../src/tabbar/Helpdesk.png'), require('../../src/tabbar/Helpdesk.png'))
    },
    Calendar: {
        screen: CalendarScreen,
        path: '/calendar',
        navigationOptions: navOptionsFactory.create('Calendar', require('../../src/tabbar/Calendar.png'), require('../../src/tabbar/Calendar.png'))
    },
    Profile: {
        screen: ProfileScreen,
        path: '/profile',
        navigationOptions: navOptionsFactory.create('Profile', require('../../src/tabbar/Profile.png'), require('../../src/tabbar/Profile.png'))
    }
};

export default tabbarModel; 
