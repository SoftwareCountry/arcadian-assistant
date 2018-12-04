import React from 'react';
import Style from '../layout/style';
import { StyleSheet } from 'react-native';

//----------------------------------------------------------------------------
export const stackNavigatorConfig = {
    defaultNavigationOptions: {
        headerTitleStyle: Style.navigation.title,
        headerStyle: Style.navigation.header,
        headerTintColor: Style.color.white,
    }
};
