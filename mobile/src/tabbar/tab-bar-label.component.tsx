import React from 'react';
import { StyledText } from '../override/styled-text';
import tabBarStyles from './tab-bar-styles';

//============================================================================
export interface TabBarLabelProps {
    label: string;
    isFocused: boolean;
}

//============================================================================
export class TabBarLabel extends React.Component<TabBarLabelProps> {
    //----------------------------------------------------------------------------
    public render(): React.ReactNode {

        return (
            <StyledText numberOfLines={1} ellipsizeMode={'tail'}
                        style={this.props.isFocused ? tabBarStyles.tabBarSelectedLabel : tabBarStyles.tabBarLabel}>
                {this.props.label}
            </StyledText>
        );
    }
}
