import { ActionsObservable } from 'redux-observable';
import { LoadFailedError } from '../errors/errors.action';
import { Alert } from 'react-native';
import { startLoginProcess, StartLoginProcess } from '../auth/auth.action';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs';
import { refresh } from '../refresh/refresh.action';

export const loadFailedErrorEpic$ = (action$: ActionsObservable<LoadFailedError>) =>
    action$.ofType('LOAD-FAILED-ERROR')
        .map(({ errorMessage }) => {       
            return new Promise(resolve => {
                Alert.alert(
                    'Error',
                    `error occurred: ${errorMessage}`,
                    [{ text: 'Try again', onPress: () => resolve(refresh())}]);
            });
        })
        .flatMap(x => x);