import { NavigationRouteConfigMap, StackNavigator } from 'react-navigation';
import { HomeProfileScreen } from './home-profile-screen';
import { PeopleDepartment } from '../people/people-department';
import { stackNavigatorConfig } from '../override/stack-navigator-config';

const routeConfig: NavigationRouteConfigMap = {
    PeopleHomeScreen: {
        screen: HomeProfileScreen,
        path: '/',
        navigationOptions: {
            header: null,
        }
    },
    CurrentDepartment: {
        screen: PeopleDepartment,
        path: '/current-department'
    }
};

export const ProfileScreen = StackNavigator(routeConfig, stackNavigatorConfig);