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
