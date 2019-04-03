import React from 'react';
import Style from '../layout/style';
import { StackNavigatorConfig } from 'react-navigation';

//----------------------------------------------------------------------------
export const defaultStackNavigatorConfig: StackNavigatorConfig = {
    defaultNavigationOptions: {
        headerTitleStyle: Style.navigation.title,
        headerStyle: Style.navigation.header,
        headerTintColor: Style.color.white,
        headerBackTitle: null,
    }
};

//----------------------------------------------------------------------------
export const transparentHeaderStackNavigatorConfig: StackNavigatorConfig = {
    defaultNavigationOptions: {
        headerTransparent: true,
    }
};
