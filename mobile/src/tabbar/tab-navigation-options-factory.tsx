import React from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { Dimensions, Platform } from 'react-native';
import tabBarStyles from './tab-bar-styles';
import { ApplicationIcon, ApplicationIconBold } from '../override/application-icon';
import { TabBarLabel } from './tab-bar-label.component';

//============================================================================
export class TabNavigationOptionsFactory {

    //----------------------------------------------------------------------------
    public create(label: string, iconName: string): NavigationTabScreenOptions {

        return {
            tabBarLabel: ({ tintColor, focused }) => {
                return this.getTabBarLabel(label, focused);
            },
            tabBarIcon: ({ tintColor, focused }) => {
                if (focused) {
                    return <ApplicationIconBold
                        name={iconName}
                        size={this.applicationIconSize()}
                        style={tabBarStyles.tabImages}/>;
                } else {
                    return <ApplicationIcon
                        name={iconName}
                        size={this.applicationIconSize()}
                        style={tabBarStyles.tabImages}/>;
                }
            }
        };
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private applicationIconSize(): number {
        return Platform.OS === 'ios' ? Dimensions.get('window').width * 0.06 : Dimensions.get('window').width * 0.06;
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private getTabBarLabel(label: string, isFocused: boolean) {
        return <TabBarLabel label={label} isFocused={isFocused}/>;
    }
}
