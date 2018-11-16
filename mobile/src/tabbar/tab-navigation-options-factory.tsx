import React from 'react';
import {NavigationStateRoute, NavigationTabScreenOptions, TabScene} from 'react-navigation';
import {Dimensions, Platform} from 'react-native';
import tabBarStyles from './tab-bar-styles';
import {StyledText} from '../override/styled-text';
import {ApplicationIcon, ApplicationIconBold} from '../override/application-icon';
import {TabBarOnPress} from './tab-bar-on-press-handlers';

export class TabNavigationOptionsFactory {
    public create(label: string, iconName: string, tabBarOnPress?: TabBarOnPress): NavigationTabScreenOptions {
        return {
            tabBarLabel: this.getTabBarLabel(label),
            tabBarIcon: ({tintColor, focused}) => {
                if (focused) {
                    return <ApplicationIconBold name={iconName} size={Platform.OS === 'ios' ? Dimensions.get('window').width * 0.08 : Dimensions.get('window').width * 0.04} style={tabBarStyles.tabImages}></ApplicationIconBold>;
                } else {
                    return <ApplicationIcon name={iconName} size={Platform.OS === 'ios' ? Dimensions.get('window').width * 0.08 : Dimensions.get('window').width * 0.04} style={tabBarStyles.tabImages}></ApplicationIcon>;
                }
            },
            tabBarOnPress: tabBarOnPress,
        };
    }

    private getTabBarLabel(label: string) {
        return Platform.OS === 'ios'
            ? label
            : <StyledText numberOfLines={1} ellipsizeMode={'tail'} style={tabBarStyles.tabBarLabel}>{label}</StyledText>;  //TODO: fix text width issue on narrow screens
    }

}
