import { ActionsObservable, StateObservable } from 'redux-observable';
import { Action } from 'redux';
import { ignoreElements, map } from 'rxjs/operators';
import { NavigationActions } from 'react-navigation';
import { AppState } from '../reducers/app.reducer';
import { NavigationDependenciesContainer } from './navigation-dependencies-container';

//============================================================================
export interface NavigateToAction<T = any> extends Action<T> {
    params?: { [key: string]: any };
}

//----------------------------------------------------------------------------
export const navigationEpic = <A extends NavigateToAction>(type: A['type'], routeName: string) => {
    return (action$: ActionsObservable<A>, _: StateObservable<AppState>, dependencies: NavigationDependenciesContainer) =>
        action$.ofType(type).pipe(
            map(action => {

                const destination = {
                    routeName,
                    params: action.params,
                };

                dependencies.navigationService.dispatch(NavigationActions.navigate(destination));
            }),
            ignoreElements(),
        );
};
