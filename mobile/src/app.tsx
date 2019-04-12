/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { RootNavigator } from './tabbar/tab-navigator';
import { AppState } from './reducers/app.reducer';
import { connect } from 'react-redux';
import { NavigationContainerComponent } from 'react-navigation';
import { WelcomeScreen } from './welcome-screen/welcome-screen';
import { AuthState } from './reducers/auth/auth.reducer';
import { SplashScreen } from './splash-screen/splash-screen';
import { NavigationService } from './navigation/navigation.service';
import { Platform, StatusBar, YellowBox } from 'react-native';
import Analytics from 'appcenter-analytics';
import { getActiveRouteName } from './utils/navigation-state';
import Style from './layout/style';
import { Action, Dispatch } from 'redux';
import { DayModel } from './reducers/calendar/calendar.model';
import {
    nextCalendarPage,
    prevCalendarPage,
    resetCalendarPages,
    selectCalendarDay
} from './reducers/calendar/calendar.action';
import { loadPin, loadRefreshToken } from './reducers/auth/auth.action';

//============================================================================
interface AppDispatchProps {
    loadAuthState: () => void;
}

//============================================================================
interface AppStateProps {
    authentication: AuthState | undefined;
}

//============================================================================
interface AppOwnProps {
    navigationService: NavigationService;
}

//============================================================================
export class App extends Component<AppStateProps & AppOwnProps & AppDispatchProps> {
    //----------------------------------------------------------------------------
    public componentDidMount(): void {
        this.props.loadAuthState();

        YellowBox.ignoreWarnings(['Deserialization']);

        if (Platform.OS === 'android') {
            StatusBar.setBackgroundColor(Style.color.base);
        }
    }

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: Readonly<AppStateProps & AppOwnProps & AppDispatchProps>, nextState: Readonly<{}>, nextContext: any): boolean {
        const authentication = this.props.authentication;
        const nextAuthentication = nextProps.authentication;

        if (!authentication || !nextAuthentication) {
            return true;
        }

        if (!authentication.authInfo || !nextAuthentication.authInfo) {
            return true;
        }

        const authInfo = authentication.authInfo;
        const nextAuthInfo = nextAuthentication.authInfo;

        return authInfo.isAuthenticated !== nextAuthInfo.isAuthenticated ||
               authentication.pinCode !== nextAuthentication.pinCode;
    }

    //----------------------------------------------------------------------------
    public render() {
        const authentication = this.props.authentication;

        if (!authentication || !authentication.authInfo ||
            !authentication.authInfo.isAuthenticated || !authentication.pinCode) {
            return <SplashScreen/>;
        }

        return (
            <RootNavigator
                ref={(navigationRef: NavigationContainerComponent) => {
                    this.props.navigationService.setNavigatorRef(navigationRef);
                    console.log('navigationRef:' + navigationRef);
                }}
                onNavigationStateChange={(prevState, currentState) => {
                    const currentScreen = getActiveRouteName(currentState);
                    const prevScreen = getActiveRouteName(prevState);

                    if (prevScreen !== currentScreen) {
                        // noinspection JSIgnoredPromiseFromCall
                        Analytics.trackEvent('Navigation', { Route: currentScreen });
                    }
                }}/>
        );
    }
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState) => ({
    authentication: state.authentication,
});

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>): AppDispatchProps => ({
    loadAuthState: () => {
        dispatch(loadPin());
        dispatch(loadRefreshToken());
    },
});

export const AppWithNavigationState = connect(stateToProps, dispatchToProps)(App);

