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

//============================================================================
interface AppStateProps {
    authentication: AuthState | undefined;
}

//============================================================================
interface AppOwnProps {
    navigationService: NavigationService;
}

//============================================================================
export class App extends Component<AppStateProps & AppOwnProps> {
    //----------------------------------------------------------------------------
    public componentDidMount(): void {
        YellowBox.ignoreWarnings(['Deserialization']);
        
        if (Platform.OS === 'android') {
            StatusBar.setBackgroundColor(Style.color.base);
        }
    }

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: Readonly<AppStateProps & AppOwnProps>, nextState: Readonly<{}>, nextContext: any): boolean {
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

export const AppWithNavigationState = connect(stateToProps)(App);

