import React from 'react';
import Style from '../layout/style';
import { StackNavigatorConfig } from 'react-navigation';

//----------------------------------------------------------------------------
export const stackNavigatorConfig: StackNavigatorConfig = {
    defaultNavigationOptions: {
        headerTitleStyle: Style.navigation.title,
        headerStyle: Style.navigation.header,
        headerTintColor: Style.color.white,
        headerBackTitle: null,
    }
};
