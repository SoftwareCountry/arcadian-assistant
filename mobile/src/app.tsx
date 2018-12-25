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
import { YellowBox } from 'react-native';
import { Dispatch } from 'redux';
import { AuthActions, startLoginProcess } from './reducers/auth/auth.action';
import Analytics from 'appcenter-analytics';
import { getActiveRouteName } from './utils/navigation-state';

//============================================================================
interface AppStateProps {
    authentication: AuthState | undefined;
}

//============================================================================
interface AppOwnProps {
    navigationService: NavigationService;
}

//============================================================================
interface AppDispatchProps {
    login: () => void;
}

//============================================================================
export class App extends Component<AppStateProps & AppDispatchProps & AppOwnProps> {
    //----------------------------------------------------------------------------
    public componentDidMount(): void {
        YellowBox.ignoreWarnings(['Deserialization']);

        if (!this.props.authentication || !this.props.authentication.authInfo) {
            this.props.login();
        }
    }

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: Readonly<AppStateProps & AppDispatchProps & AppOwnProps>, nextState: Readonly<{}>, nextContext: any): boolean {
        if (!this.props.authentication || !nextProps.authentication) {
            return true;
        }

        if (!this.props.authentication.authInfo || !nextProps.authentication.authInfo) {
            return true;
        }

        const authInfo = this.props.authentication.authInfo;
        const nextAuthInfo = nextProps.authentication.authInfo;

        return authInfo.isAuthenticated !== nextAuthInfo.isAuthenticated;
    }

    //----------------------------------------------------------------------------
    public render() {
        const authentication = this.props.authentication;

        if (!authentication || !authentication.authInfo) {
            return <SplashScreen/>;
        }

        if (!authentication.authInfo.isAuthenticated) {
            return <WelcomeScreen/>;
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
const mapDispatchToProps = (dispatch: Dispatch<AuthActions>): AppDispatchProps => ({
    login: () => {
        dispatch(startLoginProcess());
    }
});

export const AppWithNavigationState = connect<AppStateProps, AppDispatchProps, AppOwnProps, AppState>(stateToProps, mapDispatchToProps)(App);

