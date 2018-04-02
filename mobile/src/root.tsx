import React, { Component } from 'react';
import { OAuthManager } from './auth/oauth-manager';
import { OAuthProcess } from './auth/oauth-process';
import { Provider } from 'react-redux';
import { storeFactory } from './reducers/app.reducer';
import { AppWithNavigationState } from './app';

import config from './config';

export class Root extends Component<{}> {
    private oauthProcess: OAuthProcess;

    constructor(props: {}, ctx?: any) {
      super(props, ctx);
      const oauthManager = new OAuthManager();
      this.oauthProcess = oauthManager.start(
        config.oauth.clientId,
        config.oauth.tenant,
        config.oauth.redirectUri);

        /*
      this.oauthProcess.forgetUser()
        .then(x => this.oauthProcess.login())
        .catch(console.error);
*/

      this.oauthProcess.login().catch(console.error);
      //this.oauthProcess.authenticationState.forEach(console.log);
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