import { RootNavigator } from '../layout/root-navigator';
import { NavigationAction, NavigationState } from 'react-navigation';

export const navigationReducer = (state: NavigationState, action: NavigationAction) => {
    const nextState = RootNavigator.router.getStateForAction(action, state);
    return nextState || state;
};