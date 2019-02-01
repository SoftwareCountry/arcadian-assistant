import { ActionsObservable } from 'redux-observable';
import { StartLoginProcess, AuthActionType, userLoggedIn, startLogoutProcess, StartLogoutProcess, startLoginProcess } from './auth.action';
import { DependenciesContainer } from '../app.reducer';
import { flatMap } from 'rxjs/operators';
import { Alert } from 'react-native';

//----------------------------------------------------------------------------
export const startLoginProcessEpic$ = (action$: ActionsObservable<StartLoginProcess>, _: unknown, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.startLoginProcess).pipe(
        flatMap(() =>
            dep.oauthProcess.login().then(
                () => userLoggedIn() //successful login
            ).catch(async e => {
                if (isCancellationError(e)) {
                    // if user clicked cancel, just set up logout state of application
                    return startLogoutProcess(true);
                }

                console.warn('Error while trying to login', e);
                return showLoginErrorDialog(e);
            })
        )
    );

function showLoginErrorDialog(e: any): Promise<StartLoginProcess | StartLogoutProcess> {
    return new Promise(resolve => {
        Alert.alert(
            'Authentication error',
            getErrorMessage(e),
            [
                {
                    text: 'Logout',
                    onPress: () => resolve(startLogoutProcess(true)),
                    style: 'cancel',
                },
                {
                    text: 'Try again',
                    onPress: () => resolve(startLoginProcess()),
                },
            ]);
    });
}

const cancellationErrorCode = '1';
const networkErrorStatus = 0;

function isCancellationError(error: any): boolean {
    const errorCode = error.code;

    return errorCode && errorCode === cancellationErrorCode;
}

function isNetworkError(error: any): boolean {
    const errorStatus = error.status;

    return (errorStatus !== undefined && errorStatus === networkErrorStatus);
}

//----------------------------------------------------------------------------
function getErrorMessage(error: any): string {
    if (isNetworkError(error)) {
        return 'Authentication server is not available, try again later';
    }

    const detailedDescription = error && error.response && error.response.error_description ?
        error.response.error_description : undefined;

    const errorText =
        error
            ? error.message
                ? error.message.toString()
                : error.toString()
            : 'unknown error';

    return detailedDescription ? detailedDescription : errorText;
}