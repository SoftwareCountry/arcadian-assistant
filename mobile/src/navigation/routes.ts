import { List } from 'immutable';

export const routeComponents = (navigationState: any): List<string> => {
    const result = [navigationState.routeName];
    while (navigationState.routes) {
        const route = navigationState.routes[navigationState.index];
        result.push(route.routeName);
        navigationState = route;
    }

    return List(result);
};
