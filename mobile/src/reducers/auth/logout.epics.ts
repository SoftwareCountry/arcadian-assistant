import { Alert } from 'react-native';
import { flatMap, map } from 'rxjs/operators';
import { from, of, empty } from 'rxjs';
import { ActionsObservable, StateObservable } from 'redux-observable';
import { StartLogoutProcess, AuthActionType, userLoggedOut } from './auth.action';
import { DependenciesContainer, AppState } from '../app.reducer';

//----------------------------------------------------------------------------
export const startLogoutProcessEpic$ = (action$: ActionsObservable<StartLogoutProcess>, state$: StateObservable<AppState>, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.startLogoutProcess).pipe(
        map(x => {
            const logoutCallback = async () => {
                await logout(dep);
                return true;
            };

            if (x.force) {
                return logoutCallback();
            }
            return showAlert(
                'Are you sure you want to logout?',
                'Logout',
                'Cancel',
                logoutCallback,
                async () => false
            );
        }),
        flatMap(x => from(x)),
        flatMap(x => {
            if (x) {
                return of(userLoggedOut());
            } else {
                return empty();
            }
        })
    );


//----------------------------------------------------------------------------
async function logout(dependencies: DependenciesContainer) {
    try {
        await dependencies.oauthProcess.logout();
        await dependencies.storage.setPin(null);
    } catch (e) {
        console.warn('Error during logout', e);
    }
}

//----------------------------------------------------------------------------
function showAlert<TOk, TCancel>(message: string, okButtonTitle: string, rejectButtonTitle: string, okButton: () => Promise<TOk>, rejectButton: () => Promise<TCancel>) {

    return new Promise<TOk | TCancel>((resolve) => {
        Alert.alert(
            'Confirmation',
            `${message}`,
            [
                {
                    text: rejectButtonTitle,
                    onPress: () => resolve(rejectButton()),
                    style: 'cancel',
                },
                {
                    text: okButtonTitle,
                    onPress: () => resolve(okButton()),
                }
            ]);
    });
}
