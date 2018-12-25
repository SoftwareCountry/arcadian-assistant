/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { NavigationState } from 'react-navigation';

//----------------------------------------------------------------------------
export function getActiveRouteName(navigationState: NavigationState): string {
    // react-navigation doesn't provide typings there
    // because it is a complex nested structure
    const route: any = navigationState.routes[navigationState.index];

    if (route.routes) {
        return `${route.routeName}/${getActiveRouteName(route)}`;
    }
    return route.routeName;
}
