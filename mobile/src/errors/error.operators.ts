/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Alert } from 'react-native';
import { EMPTY, iif, Observable, pipe, throwError, timer, UnaryFunction } from 'rxjs';
import { catchError, concatMap, exhaustMap, retryWhen } from 'rxjs/operators';
import { logHttpError } from '../utils/analytics';

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
export function getHttpErrorMessage(error: any, customErrorMessage?: string): string {
    const errorDescription = error.status ? `HTTP ${error.status}` : error.toString();
    let errorMessage = 'Unknown error occurred';
    if (customErrorMessage) {
        errorMessage = `An error occurred (${errorDescription}). ${customErrorMessage}`;
    } else if (error.status === 401) {
        errorMessage = 'Authentication failed';
    } else if (error.status === 403) {
        errorMessage = 'Authorization error. Please contact administrator';
    } else if (error.status === 503) {
        errorMessage = 'Server is not available. Please try again later';
    } else if (error.status === 0) {
        errorMessage = 'Cannot establish a connection to the server';
    } else if (errorDescription.includes('TimeoutError')) {
        errorMessage = 'Unable to connect to server. Please try again later';
    } else {
        errorMessage = `An error occurred (${errorDescription}). Please contact administrator`;
        logHttpError(error);
    }
    return errorMessage;
}

//============================================================================
function retryWhenErrorOccurred<T>(isForceLogout: boolean = false, customErrorMessage: string | undefined = undefined): UnaryFunction<Observable<T>, Observable<T>> {
    let okButtonTitle = 'Try again';
    let rejectButtonTitle = isForceLogout ? 'Logout' : 'Cancel';
    return retryWhen(errors => {
        return errors.pipe(
            exhaustMap(e => new Promise((resolve, reject) => {
                const errorMessage = getHttpErrorMessage(e, customErrorMessage);
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
export function retryDelayed<T>(maxRetries: number = 3, delay: number = 30): UnaryFunction<Observable<T>, Observable<T>> {
    return <T>(source: Observable<T>) =>
        source.pipe(
            retryWhen<T>(errors =>
                errors.pipe(
                    concatMap((error, i) => {
                        logHttpError(error);
                        return iif(
                            () => i < maxRetries,
                            timer(delay * 1000),
                            throwError(error)
                        );
                    })
                )
            )
        );
}

//============================================================================
export function handleHttpErrorsWithDefaultValue<T>(defaultValue: Observable<T>): UnaryFunction<Observable<T>, Observable<T>> {
    return handleHttpErrors(true, undefined, defaultValue);
}
