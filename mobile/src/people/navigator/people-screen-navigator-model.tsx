import { TabNavigatorConfig, NavigationRouteConfigMap } from 'react-navigation';
import React from 'react';
import { PeopleRoomFiltered, PeopleDepartmentFiltered, PeopleCompanyFiltered } from '../people-filtered';
import { TabNavigationOptionsFactory } from '../../tabbar/tab-navigation-options-factory';
import { CompanyDepartments } from '../company-departments';

const navOptionsFactory = new TabNavigationOptionsFactory();

const peopleScreenNavigatorModel: NavigationRouteConfigMap = {

    Department: {
        screen: PeopleDepartmentFiltered,
        path: '/people/department',
        navigationOptions: navOptionsFactory.create('Department', null)
    },
    Room: {
        screen: PeopleRoomFiltered,
        path: '/people/room',
        navigationOptions: navOptionsFactory.create('Room', null)
    },
    Company: {
        screen: PeopleCompanyFiltered,
        path: '/people/company',
        navigationOptions: navOptionsFactory.create('Company', null)
    }
};

export default peopleScreenNavigatorModel;
