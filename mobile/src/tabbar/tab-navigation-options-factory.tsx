import React from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import Icon from 'react-native-vector-icons/Ionicons';
import { Image, Text, Platform } from 'react-native';
import { ImageURISource } from 'react-native';
import tabBarStyles from './tab-bar-styles';


export class TabNavigationOptionsFactory {
    public create(label: string, focusedPath: ImageURISource, unfocusedPath: ImageURISource): NavigationTabScreenOptions {
        return {
            tabBarLabel: this.getTabBarLabel(label),
            tabBarIcon: ({ tintColor, focused }) =>
                <Image
                    source={focused ? focusedPath : unfocusedPath}
                    style={tabBarStyles.tabImages}
                />
        };
    }

    private getTabBarLabel(label: string) {
        return Platform.OS === 'ios'
            ? label
            : <Text numberOfLines={1} ellipsizeMode={'tail'} style={tabBarStyles.tabBarLabel}>{label}</Text>;  //TODO: fix text width issue on narrow screens
    }
}