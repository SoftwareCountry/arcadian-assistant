import { RootNavigator } from '../tabbar/tab-navigator';
import { NavigationAction, NavigationState, NavigationActions } from 'react-navigation';
import { PeopleScreenNavigator } from '../people/navigator/people-screen-navigator';

export const navigationReducer = (state: NavigationState, action: NavigationAction) => {
    const nextState: NavigationState = RootNavigator.router.getStateForAction(action, state);
    switch (action.type) {
        case NavigationActions.NAVIGATE:
            const toNavigate: NavigationState = nextState.routes.find(e => e.isTransitioning);
            if (toNavigate) {
                const length = toNavigate.routes.length;
                const lastRoute = length >= 2 ? toNavigate.routes[length - 2].routeName : undefined;
                if (state && lastRoute && action.routeName === lastRoute) {
                    return state;
                }
            }
        default:
            return nextState;
    }
};

export const navigationMiddlewareKeyReducer = (state: string = '') => {
    return state;
};