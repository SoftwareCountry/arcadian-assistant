import React, { Component } from 'react';
import { StyleSheet, View } from 'react-native';

import { RootNavigator } from './layout/root-navigator';
import { AppState, storeFactory } from './reducers/app.reducer';
import { connect, Provider } from 'react-redux';
import { addNavigationHelpers } from 'react-navigation';

export class App extends Component<any> {
  public render() {
    return <RootNavigator navigation={addNavigationHelpers({
      dispatch: this.props.dispatch,
      state: this.props.nav
    })} />;
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