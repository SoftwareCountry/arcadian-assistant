const config = Object.freeze({

    apiUrl: 'https://arcadia-assistant-dev.arcadialab.ru/api',

    oauth: Object.freeze({
        redirectUri: 'arcadia-assistant://on-login',
        clientId: 'a2ccb221-60e2-47b8-b28c-bf88a59f7f4a',
        tenant: 'fa4e9c1f-6222-443d-a083-28f80c1ffefc'
        //clientId: 'bb342a9b-5cbf-4458-aa1f-88712719774f'
        //tenant: '55b8f7f0-a315-44b6-86b7-b8a3fd144789'
    })
});

export default config;
