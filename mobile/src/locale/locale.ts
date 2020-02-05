/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { NativeModules, Platform } from 'react-native';
import * as moment from 'moment';
import 'moment/locale/en-gb';
import { Optional } from 'types';

//----------------------------------------------------------------------------
export function applyCalendarLocale() {
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
    let locale: Optional<string> = undefined;

    switch (Platform.OS) {
        case 'android':
            locale = NativeModules.I18nManager.localeIdentifier;
            break;
        case 'ios':
            locale = NativeModules.SettingsManager.settings.AppleLocale;
            if (locale === undefined) {
                locale = NativeModules.SettingsManager.settings.AppleLanguages[0];
            }
            break;
    }

    if (locale === undefined) {
        return 'en';
    }
    return locale.toLowerCase();
}
