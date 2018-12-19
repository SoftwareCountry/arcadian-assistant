export interface Configuration {
    apiUrl: string;
    oauth: {
        redirectUrl: string;
        clientId: string;
        tenant: string;
    };
}