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
import { DeepLinking } from './navigation/deep-linking';
import { OAuthManager } from './auth/oauth-manager';
import { OAuthProcess } from './auth/oauth-process';

interface AppProps {
  dispatch: Dispatch<any>;
  nav: NavigationState;
}

//TODO: move to a configuration
const redirectUri = 'arcadia-assistant://on-login';
const clientId = 'bb342a9b-5cbf-4458-aa1f-88712719774f';
const tenant = '55b8f7f0-a315-44b6-86b7-b8a3fd144789';

export class App extends Component<AppProps> {

  private oauthProcess: OAuthProcess;

  public componentDidMount() {
    BackHandler.addEventListener('hardwareBackPress', this.onBackPress );

    Linking.addEventListener('url', this.onApplicaitonLinkOpened);
    Linking.getInitialURL().then(url => this.onApplicaitonLinkOpened( { url } ));

    //initial state
    this.props.dispatch(loadUser());
    this.props.dispatch(loadDepartments());
    this.props.dispatch(loadFeeds());

    const oauthManager = new OAuthManager();
    this.oauthProcess = oauthManager.start(clientId, tenant, redirectUri);
    
    this.oauthProcess.login();
    this.oauthProcess.jwtToken.forEach(console.log);
  }

  public componentWillUnmount() {

    if (this.oauthProcess) {
      this.oauthProcess.dispose();
    }

    BackHandler.removeEventListener('hardwareBackPress', this.onBackPress);
    Linking.removeEventListener('url', this.onApplicaitonLinkOpened);
  }

  public render() {
    return <RootNavigator navigation={addNavigationHelpers({
      dispatch: this.props.dispatch as any,
      state: this.props.nav
    })} />;
  }

  private onApplicaitonLinkOpened = (e: {url: string}) => {
    const { dispatch } = this.props;
    const dl = new DeepLinking(dispatch, this.oauthProcess);
    dl.openUrl(e.url);
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
  nav: state.nav
});

const AppWithNavigationState = connect(mapStateToProps)(App);

export class Root extends Component<{}> {
  public render() {
    return (
      <Provider store={storeFactory()}>
        <AppWithNavigationState/>
      </Provider>
    );
  }
}