import { TabNavigatorConfig, NavigationRouteConfigMap } from 'react-navigation';
import React from 'react';
import { PeopleRoomFiltered, PeopleDepartmentFiltered, PeopleCompanyFiltered } from '../people-filtered';
import { TabNavigationOptionsFactory } from '../../tabbar/tab-navigation-options-factory';
import { CompanyDepartments } from '../company-departments';
import {peopleTabBarOnPressHandler} from '../../tabbar/tab-bar-on-press-handlers';

const navOptionsFactory = new TabNavigationOptionsFactory();

const peopleScreenNavigatorModel: NavigationRouteConfigMap = {

    Department: {
        screen: PeopleDepartmentFiltered,
        path: '/people/department',
        navigationOptions: navOptionsFactory.create('Department', null, peopleTabBarOnPressHandler)
    },
    Room: {
        screen: PeopleRoomFiltered,
        path: '/people/room',
        navigationOptions: navOptionsFactory.create('Room', null, peopleTabBarOnPressHandler)
    },
    Company: {
        screen: PeopleCompanyFiltered,
        path: '/people/company',
        navigationOptions: navOptionsFactory.create('Company', null)
    }
};

export default peopleScreenNavigatorModel;
