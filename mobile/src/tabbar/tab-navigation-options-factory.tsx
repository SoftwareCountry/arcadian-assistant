import React from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import {Platform, Dimensions } from 'react-native';
import tabBarStyles from './tab-bar-styles';
import { StyledText } from '../override/styled-text';
import { ApplicationIcon, ApplicationIconBold } from '../override/application-icon';

//============================================================================
export class TabNavigationOptionsFactory {

    //----------------------------------------------------------------------------
    public create(label: string, iconName: string): NavigationTabScreenOptions {
        return {
            tabBarLabel: this.getTabBarLabel(label),
            tabBarIcon: ({ tintColor, focused }) => {
                if (focused) {
                    return <ApplicationIconBold
                        name={iconName}
                        size={Platform.OS === 'ios' ? Dimensions.get('window').width * 0.08 : Dimensions.get('window').width * 0.04}
                        style={tabBarStyles.tabImages}/>;
                } else {
                    return <ApplicationIcon
                        name={iconName}
                        size={Platform.OS === 'ios' ? Dimensions.get('window').width * 0.08 : Dimensions.get('window').width * 0.04}
                        style={tabBarStyles.tabImages}/>;
                }
            }

        };
    }

    //----------------------------------------------------------------------------
    private getTabBarLabel(label: string) {
        return Platform.OS === 'ios'
            ? label
            : <StyledText numberOfLines={1} ellipsizeMode={'tail'} style={tabBarStyles.tabBarLabel}>{label}</StyledText>;  //TODO: fix text width issue on narrow screens
    }
}
