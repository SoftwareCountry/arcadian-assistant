import { RootNavigator } from '../tabbar/tab-navigator';
import { NavigationAction, NavigationState } from 'react-navigation';
import { TopTabBarNavigator } from '../toptabbar/top-tab-bar-navigator';

export const navigationReducer = (state: NavigationState, action: NavigationAction) => {
    const nextState = RootNavigator.router.getStateForAction(action, state);
    return nextState || state;
};

export const peopleNavigationReducer = (state: NavigationState, action: NavigationAction) => {
    const nextState = TopTabBarNavigator.router.getStateForAction(action, state);
    return nextState || state;
};