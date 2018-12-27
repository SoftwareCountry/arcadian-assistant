/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import Analytics from 'appcenter-analytics';

//----------------------------------------------------------------------------
export function logHttpError(error: any) {
    let properties: { [name: string]: string } = {};
    if (error.status) {
        properties['Status'] = `${error.status}`;
    }
    if (error.request) {
        properties['URL'] = error.request.url;
        properties['Method'] = error.request.method;
        properties['Body'] = JSON.stringify(error.request.body);
    }
    Analytics.trackEvent('Error', properties);
}

//----------------------------------------------------------------------------
export function logError(errorMessage: string, payload: any) {
    let properties: { [name: string]: string } = {};
    properties['Message'] = errorMessage;
    properties['Payload'] = JSON.stringify(payload);
    Analytics.trackEvent('Error', properties);
}
