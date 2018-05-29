import { ActionsObservable } from 'redux-observable';
import { LoadFailedError } from '../errors/errors.action';
import { Alert } from 'react-native';
import { startLoginProcess, StartLoginProcess, startLogoutProcess } from '../auth/auth.action';
import { Observable } from 'rxjs/Observable';
import { Subject, AjaxError, pipe } from 'rxjs';
import { refresh } from '../refresh/refresh.action';
import { UnaryFunction } from 'rxjs/interfaces';
import { retryWhen, catchError } from 'rxjs/operators';

function showAlert(errorMessage: string, okButtonTitle: string, rejectButtonTitle: string, okButton: () => void, rejectButton: () => void) {
    Alert.alert(
        'Error',
        `${errorMessage}`,
        [{ text: okButtonTitle, onPress: () => okButton() }, { text: rejectButtonTitle, onPress: () => rejectButton() }]);
}

function retryWhenErrorOccured<T>(isForceLogout: boolean = false): UnaryFunction<Observable<T>, Observable<T>> {
    let errorMessage = 'Uknown error occured';
    let okButtonTitle = 'Try again';
    let regectButtonTitle = isForceLogout ? 'Logout' : 'Cancel';
    return retryWhen(errors => {

        return errors.exhaustMap((e: any) => new Promise((resolve, reject) => {
            if (e.status === 401 || e.status === 403) {
                errorMessage = 'Authentication failed';
                showAlert(errorMessage, okButtonTitle, regectButtonTitle,  resolve, () => reject(e));

            } else if (e.status === 0) {
                errorMessage = 'Cannot establish a connection to the server';
                showAlert(errorMessage, okButtonTitle, regectButtonTitle, resolve, () => reject(e));
            } else {
                errorMessage = `Unknown error occurred ${e}. Please contact administrator`;
                showAlert(errorMessage, okButtonTitle, regectButtonTitle, resolve, () => reject(e));
            }
        }));
    });
}

export function handleHttpErrors<T>(swallowErrors: boolean = true): UnaryFunction<Observable<T>, Observable<T>> {
    let errorMessage = 'Uknown error occured';
    if (swallowErrors) {
        return pipe(
            retryWhenErrorOccured(),
            catchError(e => { console.warn(e); return Observable.empty<T>(); })
        );
    } else {
        return pipe(
            retryWhenErrorOccured(true)
        );
    }
}

export const loadFailedErrorEpic$ = (action$: ActionsObservable<LoadFailedError>) =>
    action$.ofType('LOAD-FAILED-ERROR')
        .map(({ errorMessage }) => {
            return new Promise(resolve => {
                Alert.alert(
                    'Error',
                    `error occurred: ${errorMessage}`,
                    [{ text: 'Try again', onPress: () => resolve(refresh()) }]);
            });
        })
        .flatMap(x => x);