import { ActionsObservable } from 'redux-observable';
import { ErrorLoadFailed } from '../errors/errors.action';
import { Alert } from 'react-native';

export const errorLoadFailedEpic$ = (action$: ActionsObservable<ErrorLoadFailed>) =>
action$.ofType('ERROR-LOAD-FAILED')
    .do(({ errorMessage }) => Alert.alert('Error', `error occurred: ${errorMessage}`))
    .ignoreElements();
    