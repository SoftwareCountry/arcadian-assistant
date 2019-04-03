import { createStackNavigator, NavigationRouteConfigMap } from 'react-navigation';
import { HomeProfileScreen } from './home-profile-screen';
import { defaultStackNavigatorConfig } from '../override/stack-navigator-config';
import { CurrentPeopleDepartment } from '../people/current-people-department';
import { CurrentPeopleRoom } from '../people/current-people-room';
import { EmployeeDetailsScreen } from '../employee-details/employee-details-screen';
import { UserPreferencesScreen } from '../user-preferences-screen/user-preferences-screen';

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
    UserPreferences: {
        screen: UserPreferencesScreen,
        path: '/user-preferences',
    },
};

export const ProfileScreen = createStackNavigator(routeConfig, defaultStackNavigatorConfig);
