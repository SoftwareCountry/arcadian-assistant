import {NavigationStateRoute, TabScene} from 'react-navigation';

export type TabBarOnPress = (options: { scene: TabScene, jumpToIndex: (index: number) => void }) => void;

const firstScrollableItemInStackNavigatorTabBarOnPressHandler: TabBarOnPress = options => {
    if (options.scene.focused && options.scene.index === 0 && options.scene.route) {

        let route = options.scene.route as NavigationStateRoute<any>;

        if (route.routes && route.routes[0]) {

            const stackNavigation = route.routes[0];

            if (stackNavigation && stackNavigation.params && stackNavigation.params.tabBarOnPress) {
                let tabBarOnPress = stackNavigation.params.tabBarOnPress as () => void;
                tabBarOnPress();
            }
        }

        return;
    }

    options.jumpToIndex(options.scene.index);
};

export const feedsTabBarOnPressHandler = firstScrollableItemInStackNavigatorTabBarOnPressHandler;

export const peopleTabBarOnPressHandler: TabBarOnPress = options => {
    if (options.scene.focused && options.scene.route) {

        let route = options.scene.route as NavigationStateRoute<any>;

        if (route && route.params) {
            let tabBarOnPress = route.params.tabBarOnPress as () => void;
            tabBarOnPress();
        }

        return;
    }

    options.jumpToIndex(options.scene.index);

};

export const calendarTabBarOnPressHandler: TabBarOnPress = options => {
    if (options.scene.focused && options.scene.index === 0 && options.scene.route) {

        //todo

        return;
    }

    options.jumpToIndex(options.scene.index);

};
