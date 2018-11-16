import React, { Component } from 'react';
import { OAuthManager } from './auth/oauth-manager';
import { OAuthProcess } from './auth/oauth-process';
import { Provider } from 'react-redux';
import { storeFactory } from './reducers/app.reducer';
import { AppWithNavigationState } from './app';
import config from './config';
import { Modal, Platform, Text } from 'react-native';

export class Root extends Component<{}> {
    private oauthProcess: OAuthProcess;

    constructor(props: {}, ctx?: any) {
      super(props, ctx);
      const oauthManager = new OAuthManager();
      this.oauthProcess = oauthManager.start(
        config.oauth.clientId,
        config.oauth.tenant,
        config.oauth.redirectUri);

      const isIOS = Platform.OS === 'ios';
      if (isIOS) {
          this.oauthProcess.login().catch(console.error);
      }
    }

    public render() {
      return (
        <Provider store={storeFactory(this.oauthProcess)}>
          <AppWithNavigationState/>
        </Provider>
      );
    }

    public componentWillUnmount() {
      if (this.oauthProcess) {
        this.oauthProcess.dispose();
      }
    }
  }
