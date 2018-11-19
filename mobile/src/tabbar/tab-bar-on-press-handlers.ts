import {NavigationStateRoute, TabScene} from 'react-navigation';
import {OnSelectedDayCallback} from '../calendar/calendar-page';
import moment from 'moment';

export type TabBarOnPress = (options: { scene: TabScene, jumpToIndex: (index: number) => void }) => void;

export const feedsTabBarOnPressHandler: TabBarOnPress = options => {
    if (options.scene.focused && options.scene.route) {

        const route = options.scene.route as NavigationStateRoute<any>;

        if (route.routes && route.routes[0]) {

            const stackNavigation = route.routes[0];

            if (stackNavigation && stackNavigation.params && stackNavigation.params.tabBarOnPress) {
                const tabBarOnPress = stackNavigation.params.tabBarOnPress as () => void;
                tabBarOnPress();
            }
        }

        return;
    }

    options.jumpToIndex(options.scene.index);
};

export const peopleTabBarOnPressHandler: TabBarOnPress = options => {
    if (options.scene.focused && options.scene.route) {

        const route = options.scene.route as NavigationStateRoute<any>;

        if (route && route.params) {
            const tabBarOnPress = route.params.tabBarOnPress as () => void;
            tabBarOnPress();
        }

        return;
    }

    options.jumpToIndex(options.scene.index);

};

export const calendarTabBarOnPressHandler: TabBarOnPress = options => {
    if (options.scene.focused && options.scene.route) {

        const route = options.scene.route as NavigationStateRoute<any>;

        if (route.routes && route.routes[0]) {

            const stackNavigation = route.routes[0];

            if (stackNavigation && stackNavigation.params && stackNavigation.params.tabBarOnPress) {
                const tabBarOnPress = stackNavigation.params.tabBarOnPress as OnSelectedDayCallback;
                tabBarOnPress({
                    date: moment(), today: true, belongsToCurrentMonth: true
                });
            }
        }

        return;
    }

    options.jumpToIndex(options.scene.index);
};
