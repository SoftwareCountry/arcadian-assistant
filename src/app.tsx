import React, { Component } from 'react';
import { StyleSheet, View } from 'react-native';

import { RootNavigator } from './layout/root-navigator';
import { AppState, store } from './reducers/app.reducer';
import { connect, Provider } from 'react-redux';

export class App extends Component {
  public render() {
    return <RootNavigator />;
  }
}

export class Root extends Component<{}> {
  public render() {
    return (
      <Provider store={store}>
        <App/>
      </Provider>
    );
  }
}