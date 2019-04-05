import React from 'react';
import Style from '../layout/style';
import { StackNavigatorConfig } from 'react-navigation';
import { BackNavigationArrow } from '../navigation/back/back-navigation-arrow';

//----------------------------------------------------------------------------
export const stackNavigatorConfig: StackNavigatorConfig = {
    defaultNavigationOptions: {
        headerTitleStyle: Style.navigation.title,
        headerStyle: Style.navigation.header,
        headerBackTitle: null,
        headerBackImage: <BackNavigationArrow/>,
    },
};
