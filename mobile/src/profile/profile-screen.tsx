import { NavigationRouteConfigMap, StackNavigator } from 'react-navigation';
import { HomeProfileScreen } from './home-profile-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { CurrentPeopleDepartment } from '../people/current-people-department';

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
    }
};

export const ProfileScreen = StackNavigator(routeConfig, stackNavigatorConfig);