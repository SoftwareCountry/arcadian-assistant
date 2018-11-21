import React from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { Dimensions, Platform } from 'react-native';
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
        return Platform.OS === 'ios' ? Dimensions.get('window').width * 0.08 : Dimensions.get('window').width * 0.04;
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private getTabBarLabel(label: string) {

        const labelStyled =
            <StyledText numberOfLines={1} ellipsizeMode={'tail'} style={tabBarStyles.tabBarLabel}>
                {label}
            </StyledText>; //TODO: fix text width issue on narrow screens

        return Platform.OS === 'ios' ? label : labelStyled;
    }
}
