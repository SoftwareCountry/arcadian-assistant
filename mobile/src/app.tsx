import React, { Component } from 'react';
import { StyleSheet, View, BackHandler } from 'react-native';

import { RootNavigator } from './tabbar/tab-navigator';
import { AppState, storeFactory } from './reducers/app.reducer';
import { connect, Provider, Dispatch } from 'react-redux';
import { addNavigationHelpers, NavigationState, NavigationActions } from 'react-navigation';
import { loadDepartments, loadUser } from './reducers/organization/organization.action';

interface AppProps {
  dispatch: Dispatch<any>;
  nav: NavigationState;
}

export class App extends Component<AppProps> {

  public componentDidMount() {
    BackHandler.addEventListener('hardwareBackPress', this.onBackPress );

    //initial state
    this.props.dispatch(loadUser());
    this.props.dispatch(loadDepartments());
  }

  public componentWillUnmount() {
    BackHandler.removeEventListener('hardwareBackPress', this.onBackPress);
  }

  public render() {
    return <RootNavigator navigation={addNavigationHelpers({
      dispatch: this.props.dispatch as any,
      state: this.props.nav
    })} />;
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