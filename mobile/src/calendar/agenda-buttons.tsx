import React, { Component } from 'react';
import { TouchableHighlight, TouchableOpacity, View } from 'react-native';
import { agendaButtonsStyles } from './styles';
import { StyledText } from '../override/styled-text';

export class AgendaButtons extends Component {
    public render() {
        return (
            <View style={agendaButtonsStyles.container}>
                <TouchableOpacity>
                    <StyledText>Request Vacation</StyledText>
                </TouchableOpacity>
                <TouchableOpacity>
                    <StyledText>Process Dayoff</StyledText>
                </TouchableOpacity>
                <TouchableOpacity>
                    <StyledText>Claim Sick Leave</StyledText>
                </TouchableOpacity>
            </View>
        );
    }
}