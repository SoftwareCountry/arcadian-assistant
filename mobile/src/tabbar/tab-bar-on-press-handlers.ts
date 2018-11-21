import { NavigationStateRoute, TabScene } from 'react-navigation';

export type TabBarOnPress = (options: { scene: TabScene, jumpToIndex: (index: number) => void }) => void;

const defaultTabBarOnPressHandler: TabBarOnPress = options => {
    if (options.scene.focused && options.scene.route) {

        const route = options.scene.route as NavigationStateRoute;

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

export const feedsTabBarOnPressHandler = defaultTabBarOnPressHandler;
export const calendarTabBarOnPressHandler = defaultTabBarOnPressHandler;

export const peopleTabBarOnPressHandler: TabBarOnPress = options => {
    if (options.scene.focused && options.scene.route) {

        const route = options.scene.route as NavigationStateRoute;

        if (route && route.params) {
            const tabBarOnPress = route.params.tabBarOnPress as () => void;
            tabBarOnPress();
        }

        return;
    }

    options.jumpToIndex(options.scene.index);

};
