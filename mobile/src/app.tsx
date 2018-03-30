import React, { Component } from 'react';
import { StyleSheet, View, BackHandler, Linking } from 'react-native';

import { RootNavigator } from './tabbar/tab-navigator';
import { AppState, storeFactory } from './reducers/app.reducer';
import { connect, Provider, Dispatch } from 'react-redux';
import { addNavigationHelpers, NavigationState, NavigationActions } from 'react-navigation';
import { loadDepartments } from './reducers/organization/organization.action';
import { loadFeeds } from './reducers/feeds/feeds.action';
import { Employee } from './reducers/organization/employee.model';
import { loadUser } from './reducers/user/user.action';
import { AddListener, createReduxBoundAddListener } from 'react-navigation-redux-helpers';
import { WelcomeScreen } from './welcome-screen/welcome-screen';
import { AuthState } from './reducers/auth/auth.reducer';
import { SplashScreen } from './splash-screen/splash-screen';

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
    if (this.props.authentication.isAuthenticated === true) {
      return <RootNavigator navigation={addNavigationHelpers({
        dispatch: this.props.dispatch as any,
        state: this.props.nav,
        addListener: this.addListener
      })} />;
    } else if (this.props.authentication.isAuthenticated === false) {
      return <WelcomeScreen />;
    } else {
      return <SplashScreen />;
    }
  }

  public componentDidMount() {
    BackHandler.addEventListener('hardwareBackPress', this.onBackPress );

    //initial state
    this.props.dispatch(loadUser());
    this.props.dispatch(loadDepartments());
    this.props.dispatch(loadFeeds());
  }

  public componentWillUnmount() {
    BackHandler.removeEventListener('hardwareBackPress', this.onBackPress);
  }

  private onBackPress = () => {
    const { dispatch, nav } = this.props;
    if (nav.index === 0) {
      return false;
    }

    dispatch(NavigationActions.back());
    return true;
  }
}

const mapStateToProps = (state: AppState) => ({
  nav: state.nav,
  reduxNavKey: state.navigationMiddlewareKey,
  authentication: state.authentication,
});

export const AppWithNavigationState = connect(mapStateToProps)(App);