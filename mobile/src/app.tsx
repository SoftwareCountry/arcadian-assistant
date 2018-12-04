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

//============================================================================
interface AppProps {
    authentication: AuthState | undefined;
}

//============================================================================
interface AppOwnProps {
    navigationService: NavigationService;
}

//============================================================================
export class App extends Component<AppProps & AppOwnProps> {
    //----------------------------------------------------------------------------
    public componentDidMount(): void {
        YellowBox.ignoreWarnings(['Deserialization']);
    }

    //----------------------------------------------------------------------------
    public render() {
        if (!this.props.authentication || !this.props.authentication.authInfo) {
            return (
                <SplashScreen/>
            );
        }

        if (this.props.authentication.authInfo.isAuthenticated) {
            return <RootNavigator ref={(navigationRef: NavigationContainerComponent) => {
                this.props.navigationService.setNavigatorRef(navigationRef);
                console.log('navigationRef:' + navigationRef);
            }}/>;
        } else {
            return <WelcomeScreen/>;
        }
    }
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState) => ({
    authentication: state.authentication,
});

export const AppWithNavigationState = connect<AppProps, {}, AppOwnProps>(stateToProps)(App);
