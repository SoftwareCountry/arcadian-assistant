import React, {Component} from 'react';
import {BackHandler} from 'react-native';
import {RootNavigator} from './tabbar/tab-navigator';
import {AppState} from './reducers/app.reducer';
import {connect} from 'react-redux';
import {NavigationContainerComponent} from 'react-navigation';
import {WelcomeScreen} from './welcome-screen/welcome-screen';
import {AuthState} from './reducers/auth/auth.reducer';
import {SplashScreen} from './splash-screen/splash-screen';
import { NavigationService } from './navigation/navigation.service';

interface AppProps {
    authentication: AuthState;
}

interface AppOwnProps {
    navigationService: NavigationService;
}

export class App extends Component<AppProps & AppOwnProps> {
    public render() {
        if (!this.props.authentication.authInfo) {
            return (
                <SplashScreen/>
            );
        }

        if (this.props.authentication.authInfo.isAuthenticated) {
            return <RootNavigator ref={(navigationRef: NavigationContainerComponent) => {
                this.props.navigationService.setNavigatorRef(navigationRef);
                console.log(navigationRef);
            }}/>;
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
        // const { dispatch, nav: currentState } = this.props;
        // const popAction = NavigationActions.pop({ n: 1 });
        // const nextState = RootNavigator.router.getStateForAction(popAction, currentState);
        // if (routeComponents(currentState).equals(routeComponents(nextState))) {
        //     return false;
        // }
        // dispatch(popAction);
        return true;
    };
}

const stateToProps = (state: AppState) => ({
    authentication: state.authentication,
});

export const AppWithNavigationState = connect<AppProps, {}, AppOwnProps>(stateToProps)(App);
