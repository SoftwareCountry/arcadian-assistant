import React, { Component } from 'react';
import { OAuthManager } from './auth/oauth-manager';
import { OAuthProcess } from './auth/oauth-process';
import { Provider } from 'react-redux';
import { storeFactory } from './reducers/app.reducer';
import { AppWithNavigationState } from './app';
import config from './config';
import { NavigationService } from './navigation/navigation.service';
import { Storage } from './storage/storage';

//============================================================================
export class Root extends Component<{}> {
    private readonly oauthProcess: OAuthProcess;
    private readonly navigationService: NavigationService;
    private readonly storage: Storage;

    //----------------------------------------------------------------------------
    constructor(props: {}, ctx?: any) {
        super(props, ctx);

        this.storage = new Storage();

        const oauthManager = new OAuthManager();
        this.oauthProcess = oauthManager.start(
            config.oauth.clientId,
            config.oauth.tenant,
            config.oauth.redirectUri,
            this.storage);

        this.navigationService = new NavigationService();
    }

    //----------------------------------------------------------------------------
    public render() {
        return (
            <Provider store={storeFactory(this.oauthProcess, this.navigationService, this.storage)}>
                <AppWithNavigationState navigationService={this.navigationService}/>
            </Provider>
        );
    }

    //----------------------------------------------------------------------------
    public componentWillUnmount() {
        this.navigationService.dispose();
    }
}
