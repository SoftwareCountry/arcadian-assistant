import React from 'react';
import { StyledText } from '../override/styled-text';
import tabBarStyles from './tab-bar-styles';

//============================================================================
export interface TabBarLabelProps {
    label: string;
}

//============================================================================
export class TabBarLabel extends React.Component<TabBarLabelProps> {
    //----------------------------------------------------------------------------
    public render(): React.ReactNode {
        return (
            <StyledText numberOfLines={1} ellipsizeMode={'tail'} style={tabBarStyles.tabBarLabel}>
                {this.props.label}
            </StyledText>
        );
    }
}
