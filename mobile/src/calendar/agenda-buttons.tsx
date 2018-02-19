import React, { Component } from 'react';
import { TouchableHighlight, TouchableOpacity, View } from 'react-native';
import { agendaPrimaryButtonsStyles, calendarIntervalColors } from './styles';
import { StyledText } from '../override/styled-text';
import { AgendaPrimaryButton, AgendaPrimarySeparator } from './agenda-button';

export class AgendaButtons extends Component {
    public render() {
        return (
            <View style={agendaPrimaryButtonsStyles.container}>
                <AgendaPrimaryButton title={'Request Vacation'} borderColor={calendarIntervalColors.vacation} />
                <AgendaPrimarySeparator />
                <AgendaPrimaryButton title={'Process Dayoff'} borderColor={calendarIntervalColors.dayoff} />
                <AgendaPrimarySeparator />
                <AgendaPrimaryButton title={'Claim Sick Leave'} borderColor={calendarIntervalColors.sickLeave} />
                <AgendaPrimarySeparator />
            </View>
        );
    }
}