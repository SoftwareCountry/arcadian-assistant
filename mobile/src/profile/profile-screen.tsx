import { NavigationRouteConfigMap, StackNavigator } from 'react-navigation';
import { HomeProfileScreen } from './home-profile-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { CurrentPeopleDepartment } from '../people/current-people-department';
import { CurrentPeopleRoom } from '../people/current-people-room';
import { CompanyDepartments } from '../people/company-departments';

const routeConfig: NavigationRouteConfigMap = {
    PeopleHomeScreen: {
        screen: HomeProfileScreen,
        path: '/',
        navigationOptions: {
            header: null,
        }
    },
    CurrentDepartment: {
        screen: CurrentPeopleDepartment,
        path: '/current-department'
    },
    CurrentRoom: {
        screen: CurrentPeopleRoom,
        path: '/current-room'
    }
};

export const ProfileScreen = StackNavigator(routeConfig, stackNavigatorConfig);