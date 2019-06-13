import { NavigationRouteConfigMap } from 'react-navigation';
import React from 'react';
import { FeedsScreen } from '../feeds/feeds-screen';
import { CalendarScreen } from '../calendar/calendar-screen';
import { ProfileScreen } from '../profile/profile-screen';
import { TabNavigationOptionsFactory } from './tab-navigation-options-factory';
import { PeopleScreen } from '../people/people-screen';

const navOptionsFactory = new TabNavigationOptionsFactory();

const tabBarModel: NavigationRouteConfigMap = {

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

export default tabBarModel;
