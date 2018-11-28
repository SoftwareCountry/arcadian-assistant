import React, { Component } from 'react';
import { OAuthManager } from './auth/oauth-manager';
import { OAuthProcess } from './auth/oauth-process';
import { Provider } from 'react-redux';
import { storeFactory } from './reducers/app.reducer';
import { AppWithNavigationState } from './app';
import config from './config';
import { Modal, Platform, Text } from 'react-native';
import { NavigationService } from './navigation/navigation.service';

export class Root extends Component<{}> {
    private oauthProcess: OAuthProcess;
    private navigationService: NavigationService;

    constructor(props: {}, ctx?: any) {
      super(props, ctx);
      const oauthManager = new OAuthManager();
      this.oauthProcess = oauthManager.start(
        config.oauth.clientId,
        config.oauth.tenant,
        config.oauth.redirectUri);

      this.navigationService = new NavigationService();

      const isIOS = Platform.OS === 'ios';
      if (isIOS) {
          this.oauthProcess.login().catch(console.error);
      }
    }

    public render() {
      const navigationService = this.navigationService;

      return (
        <Provider store={storeFactory(this.oauthProcess)}>
          <AppWithNavigationState navigationService={navigationService}/>
        </Provider>
      );
    }

    public componentWillUnmount() {
      if (this.oauthProcess) {
        this.oauthProcess.dispose();
      }
    }
  }
