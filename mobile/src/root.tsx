import React, { Component } from 'react';
import { OAuthManager } from './auth/oauth-manager';
import { OAuthProcess } from './auth/oauth-process';
import { Provider } from 'react-redux';
import { storeFactory } from './reducers/app.reducer';
import { AppWithNavigationState } from './app';
import config from './config';
import { NavigationService } from './navigation/navigation.service';
import { PinCodeStorage } from './auth/pin-code-storage';

export class Root extends Component<{}> {
    private readonly oauthProcess: OAuthProcess;
    private readonly navigationService: NavigationService;
    private readonly pinStorage: PinCodeStorage;

    constructor(props: {}, ctx?: any) {
        super(props, ctx);
        const oauthManager = new OAuthManager();
        this.oauthProcess = oauthManager.start(
            config.oauth.clientId,
            config.oauth.tenant,
            config.oauth.redirectUri);

        this.navigationService = new NavigationService();
        this.pinStorage = new PinCodeStorage();
    }

    public render() {
        return (
            <Provider store={storeFactory(this.oauthProcess, this.navigationService, this.pinStorage)}>
                <AppWithNavigationState navigationService={this.navigationService}/>
            </Provider>
        );
    }

    public componentWillUnmount() {
        this.navigationService.dispose();
    }
}
