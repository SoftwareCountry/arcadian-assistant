import React, { Component } from 'react';
import { Switch, View } from 'react-native';
import { preferencesStyles } from './preferences.styles';
import { StyledText } from '../override/styled-text';

interface SwitchSettingsProps {
    title: string;
    onValueChange: (value: boolean) => void;
    value: boolean;
}

export class SwitchSettingsView extends Component<SwitchSettingsProps> {

    public render() {
        return <View style={preferencesStyles.switchSettingContainer}>
            <StyledText style={preferencesStyles.switchSettingTitle}>{this.props.title}</StyledText>
            <Switch style={preferencesStyles.switchControllerContainer} onValueChange={this.props.onValueChange}
                    value={this.props.value}/>
        </View>;
    }
}
