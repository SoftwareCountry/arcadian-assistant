import { ActionsObservable } from 'redux-observable';
import { LoadFailedError } from '../errors/errors.action';
import { Alert } from 'react-native';
import { startLoginProcess, StartLoginProcess, startLogoutProcess } from '../auth/auth.action';
import { Observable } from 'rxjs/Observable';
import { Subject, AjaxError } from 'rxjs';
import { refresh } from '../refresh/refresh.action';
import { UnaryFunction } from 'rxjs/interfaces';
import { retryWhen } from 'rxjs/operators';

export function handleHttpErrors<T>(): UnaryFunction<Observable<T>, Observable<T>> {
    return retryWhen(errors => {

        return errors.exhaustMap((e: any) => new Promise(resolve => {
            let message = '';

            if (e.status === 401 || e.status === 403) {
                Alert.alert(
                    'Error',
                    'Authentication failed',
                    [{ text: 'Ok', onPress: () => resolve() }]);
            } else if (e.status === 0) {
                Alert.alert(
                    'Error',
                    `error occurred: ${e}`,
                    [{ text: 'Try again', onPress: () => resolve() }]);
            } else {
                Alert.alert(
                    'Error',
                    `error occurred: ${e}`,
                    [{ text: 'Try again', onPress: () => resolve() }]);
            }
        }));
    });
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