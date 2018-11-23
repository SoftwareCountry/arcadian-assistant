import React, { Component } from 'react';
import { BackHandler } from 'react-native';
import { RootNavigator } from './tabbar/tab-navigator';
import { AppState } from './reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { addNavigationHelpers, NavigationActions, NavigationState } from 'react-navigation';
import { AddListener, createReduxBoundAddListener } from 'react-navigation-redux-helpers';
import { WelcomeScreen } from './welcome-screen/welcome-screen';
import { AuthState } from './reducers/auth/auth.reducer';
import { SplashScreen } from './splash-screen/splash-screen';
import { routeComponents } from './navigation/routes';

interface AppProps {
    dispatch: Dispatch<any>;
    nav: NavigationState;
    reduxNavKey: string;
    authentication: AuthState;
}

export class App extends Component<AppProps> {
    private addListener: AddListener;

    public componentWillMount() {
        this.addListener = createReduxBoundAddListener(this.props.reduxNavKey);
    }

    public render() {
        if (!this.props.authentication.authInfo) {
            return (
                <SplashScreen/>
            );
        }

        if (this.props.authentication.authInfo.isAuthenticated) {
            return <RootNavigator navigation={addNavigationHelpers({
                dispatch: this.props.dispatch as any,
                state: this.props.nav,
                addListener: this.addListener
            })}/>;
        } else {
            return <WelcomeScreen/>;
        }
    }

    public componentDidMount() {
        BackHandler.addEventListener('hardwareBackPress', this.onBackPress);
    }

    public componentWillUnmount() {
        BackHandler.removeEventListener('hardwareBackPress', this.onBackPress);
    }

    private onBackPress = () => {
        const { dispatch, nav: currentState } = this.props;
        const popAction = NavigationActions.pop({ n: 1 });
        const nextState = RootNavigator.router.getStateForAction(popAction, currentState);
        if (routeComponents(currentState).equals(routeComponents(nextState))) {
            return false;
        }
        dispatch(popAction);
        return true;
    };
}

const mapStateToProps = (state: AppState) => ({
    nav: state.nav,
    reduxNavKey: state.navigationMiddlewareKey,
    authentication: state.authentication,
});

export const AppWithNavigationState = connect(mapStateToProps)(App);
