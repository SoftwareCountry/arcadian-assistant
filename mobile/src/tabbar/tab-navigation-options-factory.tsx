import React from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import Icon from 'react-native-vector-icons/Ionicons';
import { Image } from 'react-native';
import { ImageURISource } from 'react-native';


export class TabNavigationOptionsFactory {
    public create(label: string, focusedPath: ImageURISource, unfocusedPath: ImageURISource): NavigationTabScreenOptions {
        return {
            tabBarLabel: label,
            tabBarIcon: ({ tintColor, focused }) =>
                <Image
                    source={focused ? focusedPath : unfocusedPath}
                    style={{ width: 30, height: 26 }}
                />
        };
    }
}