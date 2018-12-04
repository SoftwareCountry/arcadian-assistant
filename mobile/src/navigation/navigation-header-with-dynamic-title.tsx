import {
    NavigationRoute,
    NavigationScreenConfigProps,
    NavigationScreenProp,
    NavigationStackScreenOptions
} from 'react-navigation';
import { View } from 'react-native';
import Style from '../layout/style';
import { StyledText } from '../override/styled-text';
import React from 'react';

//----------------------------------------------------------------------------
export type NavigationOptionsContainer = NavigationScreenConfigProps & {
    navigationOptions: NavigationScreenProp<NavigationRoute>;
};

//----------------------------------------------------------------------------
export function navigationOptionsWithTitle(navigationOptionsContainer: NavigationOptionsContainer, title: string): NavigationStackScreenOptions {
    return {
        ...navigationOptionsContainer.navigationOptions,
        headerTitle: <View style={Style.navigation.titleContainer}>
            <StyledText numberOfLines={1} ellipsizeMode={'tail'} style={Style.navigation.title}>{title}</StyledText>
        </View>,
        headerRight: <View/>, // to center title, otherwise it will be shifted right
    };
}
