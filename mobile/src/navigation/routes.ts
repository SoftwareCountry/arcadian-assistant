import { List } from 'immutable';
import { NavigationState } from 'react-navigation';

export const routeComponents = (navigationState: NavigationState): List<string> => {
    const result = [(navigationState as any).routeName];
    while (navigationState.routes) {
        const route = navigationState.routes[navigationState.index];
        result.push(route.routeName);
        navigationState = route;
    }

    return List(result);
};
