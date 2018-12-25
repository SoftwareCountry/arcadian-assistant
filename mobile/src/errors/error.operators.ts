/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Alert } from 'react-native';
import { EMPTY, Observable, pipe, UnaryFunction } from 'rxjs';
import { catchError, exhaustMap, retryWhen } from 'rxjs/operators';

//============================================================================
function showAlert(errorMessage: string, okButtonTitle: string, rejectButtonTitle: string, okButton: () => void, rejectButton: () => void) {
    Alert.alert(
        'Error',
        `${errorMessage}`,
        [
            {
                text: rejectButtonTitle,
                onPress: () => rejectButton(),
                style: 'cancel',
            },
            {
                text: okButtonTitle,
                onPress: () => okButton(),
            },
        ]);
}

//============================================================================
function retryWhenErrorOccurred<T>(isForceLogout: boolean = false, customErrorMessage: string | undefined = undefined): UnaryFunction<Observable<T>, Observable<T>> {
    let okButtonTitle = 'Try again';
    let rejectButtonTitle = isForceLogout ? 'Logout' : 'Cancel';
    return retryWhen(errors => {
        return errors.pipe(
            exhaustMap(e => new Promise((resolve, reject) => {
                let errorMessage = 'Unknown error occurred';
                if (customErrorMessage) {
                    errorMessage = `An error occurred ${e}. ${customErrorMessage}`;
                } else if (e.status === 401) {
                    errorMessage = 'Authentication failed';
                } else if (e.status === 403) {
                    errorMessage = 'Authorization error. Please contact administrator';
                } else if (e.status === 503) {
                    errorMessage = 'Server is not available. Please try again later';
                } else if (e.status === 0) {
                    errorMessage = 'Cannot establish a connection to the server';
                } else {
                    errorMessage = `Unknown error occurred ${e}. Please contact administrator`;
                }

                showAlert(errorMessage, okButtonTitle, rejectButtonTitle, resolve, () => reject(e));
            })));
    });
}

//============================================================================
export function handleHttpErrors<T>(swallowErrors: boolean = true,
                                    customErrorMessage: string | undefined = undefined,
                                    defaultValue: Observable<T> = EMPTY): UnaryFunction<Observable<T>, Observable<T>> {
    if (swallowErrors) {
        return pipe(
            retryWhenErrorOccurred(false, customErrorMessage),
            catchError(e => {
                console.warn(e);
                return defaultValue;
            })
        );
    } else {
        return pipe(
            retryWhenErrorOccurred(true, customErrorMessage)
        );
    }
}

//============================================================================
export function handleHttpErrorsWithDefaultValue<T>(defaultValue: Observable<T>): UnaryFunction<Observable<T>, Observable<T>> {
    return handleHttpErrors(true, undefined, defaultValue);
}
