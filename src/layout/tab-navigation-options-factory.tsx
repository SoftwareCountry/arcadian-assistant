import React from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import Icon from 'react-native-vector-icons/Ionicons';



export class TabNavigationOptionsFactory {
    create(label: string, icon: string, unfocusedIcon: string): NavigationTabScreenOptions {
        return {
            tabBarLabel: label,
            tabBarIcon: ({ tintColor, focused }) =>
                <Icon
                    name={focused ? icon : unfocusedIcon}
                    size={20}
                    style={{ color: tintColor }}
                >
                </Icon>
        }
    }
}