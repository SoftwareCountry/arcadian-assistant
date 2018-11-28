import { ActionsObservable } from 'redux-observable';
import { LoadFailedError } from './errors.action';
import { Alert } from 'react-native';
import {pipe, UnaryFunction, Observable, EMPTY } from 'rxjs';
import { refresh } from '../refresh/refresh.action';
import {retryWhen, catchError, map, flatMap, exhaustMap} from 'rxjs/operators';

function showAlert(errorMessage: string, okButtonTitle: string, rejectButtonTitle: string, okButton: () => void, rejectButton: () => void) {
    Alert.alert(
        'Error',
        `${errorMessage}`,
        [{ text: okButtonTitle, onPress: () => okButton() }, { text: rejectButtonTitle, onPress: () => rejectButton() }]);
}

function retryWhenErrorOccurred<T>(isForceLogout: boolean = false): UnaryFunction<Observable<T>, Observable<T>> {
    let okButtonTitle = 'Try again';
    let rejectButtonTitle = isForceLogout ? 'Logout' : 'Cancel';
    return retryWhen(errors => {
        return errors.pipe(
            exhaustMap(e => new Promise((resolve, reject) => {
                let errorMessage = 'Uknown error occured';
                if (e.status === 401) {
                    errorMessage = 'Authentication failed';
                } else if (e.status === 403) {
                    errorMessage = 'Authorization error. Please contact administrator';
                } else if (e.status === 0) {
                    errorMessage = 'Cannot establish a connection to the server';
                } else {
                    errorMessage = `Unknown error occurred ${e}. Please contact administrator`;
                }

                showAlert(errorMessage, okButtonTitle, rejectButtonTitle, resolve, () => reject(e));
            })));
    });
}

export function handleHttpErrors<T>(swallowErrors: boolean = true): UnaryFunction<Observable<T>, Observable<T>> {
    if (swallowErrors) {
        return pipe(
            retryWhenErrorOccurred(),
            catchError(e => { console.warn(e); return EMPTY; })
        );
    } else {
        return pipe(
            retryWhenErrorOccurred(true)
        );
    }
}

export const loadFailedErrorEpic$ = (action$: ActionsObservable<LoadFailedError>) =>
    action$.ofType('LOAD-FAILED-ERROR').pipe(
        map(({ errorMessage }) => {
            return new Promise(resolve => {
                Alert.alert(
                    'Error',
                    `error occurred: ${errorMessage}`,
                    [{ text: 'Try again', onPress: () => resolve(refresh()) }]);
            });
        }),
        flatMap(x => x));
