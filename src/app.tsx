import React, { Component } from 'react';
import { StyleSheet, View } from 'react-native';

import { RootNavigator } from './layout/root-navigator';

export default class App extends Component {
  render() {
    return <RootNavigator />;
  }
}