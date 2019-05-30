/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { NativeModules, Platform } from 'react-native';
import * as moment from 'moment';
import 'moment/locale/en-gb';

//----------------------------------------------------------------------------
export function applyLocale() {
    const defaultLocale = 'en-gb';

    const deviceLocale = getDeviceLocale();

    if (deviceLocale === 'en_us') {
        moment.locale('en');
    } else {
        moment.locale(defaultLocale);
    }
}

//----------------------------------------------------------------------------
function getDeviceLocale(): string {
    switch (Platform.OS) {
        case 'android':
            return NativeModules.I18nManager.localeIdentifier.toLowerCase();
        case 'ios':
            return NativeModules.SettingsManager.settings.AppleLocale.toLowerCase();
        default:
            return 'en';
    }
}
