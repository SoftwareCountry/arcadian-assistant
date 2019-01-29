import React, { Component } from 'react';
import { OAuthManager } from './auth/oauth-manager';
import { OAuthProcess } from './auth/oauth-process-2';
import { Provider } from 'react-redux';
import { storeFactory } from './reducers/app.reducer';
import { AppWithNavigationState } from './app';
import config from './config';
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
    }

    public render() {
        return (
            <Provider store={storeFactory(this.oauthProcess, this.navigationService)}>
                <AppWithNavigationState navigationService={this.navigationService}/>
            </Provider>
        );
    }

    public componentWillUnmount() {
        if (this.oauthProcess) {
            //this.oauthProcess.dispose(); //TODO: if dispose needed
        }

        this.navigationService.dispose();
    }
}
