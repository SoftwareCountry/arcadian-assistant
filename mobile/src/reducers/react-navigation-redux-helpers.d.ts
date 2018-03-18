declare module 'react-navigation-redux-helpers' {
    import { NavigationState, NavigationEventCallback, NavigationEventSubscription } from 'react-navigation';
    import { Middleware } from 'redux';

    type AddListener = (eventName: string, callback: NavigationEventCallback) => NavigationEventSubscription;
  
    export function createReactNavigationReduxMiddleware<S>
        (key: string, navStateSelector: (state: S) => NavigationState): Middleware;
  
    export function createReduxBoundAddListener(key: string): AddListener;
}