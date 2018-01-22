import { ActionsObservable } from 'redux-observable';
import { LoadFailedError } from '../errors/errors.action';
import { Alert } from 'react-native';

export const loadFailedErrorEpic$ = (action$: ActionsObservable<LoadFailedError>) =>
action$.ofType('LOAD-FAILED-ERROR')
    .do(({ errorMessage }) => Alert.alert('Error', `error occurred: ${errorMessage}`))
    .ignoreElements();
    