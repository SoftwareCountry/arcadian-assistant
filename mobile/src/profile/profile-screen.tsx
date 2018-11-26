import { NavigationRouteConfigMap, StackNavigator } from 'react-navigation';
import { HomeProfileScreen } from './home-profile-screen';
import { stackNavigatorConfig } from '../override/stack-navigator-config';
import { CurrentPeopleDepartment } from '../people/current-people-department';
import { CurrentPeopleRoom } from '../people/current-people-room';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';

const routeConfig: NavigationRouteConfigMap = {
    PeopleHomeScreen: {
        screen: HomeProfileScreen,
        path: '/',
    },
    CurrentDepartment: {
        screen: CurrentPeopleDepartment,
        path: '/current-department'
    },
    CurrentRoom: {
        screen: CurrentPeopleRoom,
        path: '/current-room'
    },
    CurrentProfile: {
        screen: EmployeeDetailsScreen,
        path: '/profile',
    },
};

export const ProfileScreen = StackNavigator(routeConfig, stackNavigatorConfig);
