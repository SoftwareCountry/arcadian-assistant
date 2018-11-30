import React, { Component } from 'react';
import { agendaSelectedDayStyles, agendaStyles } from './styles';
import { StyleSheet, View } from 'react-native';
import { CalendarActionsButtonGroup } from './calendar-actions-button-group';
import { CalendarLegend } from './calendar-legend';
import { SelectedDay } from './days-counters/selected-day';
import { EventDialog } from './event-dialog/event-dialog';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { EventDialogType } from '../reducers/calendar/event-dialog/event-dialog-type.model';
import { Optional } from 'types';

interface AgendaProps {
    dialogType: Optional<EventDialogType>;
}

const mapStateToProps = (state: AppState): AgendaProps => ({
    dialogType: state.calendar ? state.calendar.eventDialog.dialogType : undefined
});

export class AgendaImpl extends Component<AgendaProps> {
    public render() {
        if (!this.props.dialogType) {
            return null;
        }

        const dialogOpened = !!this.props.dialogType;

        const displayNone = StyleSheet.flatten({ display: 'none' });

        const controlsStyles = StyleSheet.flatten(dialogOpened ? displayNone : agendaStyles.controls);
        const dialogStyles = StyleSheet.flatten(dialogOpened ? agendaStyles.dialog : displayNone);

        return (
            <View style={agendaStyles.container}>
                <View style={controlsStyles}>
                    <View style={agendaSelectedDayStyles.container}>
                        <SelectedDay />
                        <CalendarLegend />
                    </View>
                    <CalendarActionsButtonGroup />
                </View>

                <View style={dialogStyles}>
                    <EventDialog dialogType={this.props.dialogType} />
                </View>
            </View>
        );
    }
}

export const Agenda = connect(mapStateToProps)(AgendaImpl);
