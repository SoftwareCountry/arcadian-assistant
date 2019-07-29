import React, { Component } from 'react';
import { agendaSelectedDayStyles, agendaStyles } from './styles';
import { View } from 'react-native';
import { ActionType, CalendarActionsButtonGroup } from './calendar-actions-button-group';
import { CalendarLegend } from './calendar-legend';
import { SelectedDay } from './days-counters/selected-day';
import { EventDialog } from './event-dialog/event-dialog';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { EventDialogType } from '../reducers/calendar/event-dialog/event-dialog-type.model';

//============================================================================
interface AgendaOwnProps {
    actions: ActionType[];
}

//============================================================================
interface AgendaProps {
    dialogType?: EventDialogType;
}

//============================================================================
export class AgendaImpl extends Component<AgendaProps & AgendaOwnProps> {
    //----------------------------------------------------------------------------
    public render() {
        return <View style={agendaStyles.container}>
            {
                this.props.dialogType ?
                    this.dialog(this.props.dialogType) :
                    this.agenda()
            }
        </View>;
    }

    //----------------------------------------------------------------------------
    private agenda(): JSX.Element {
        return <View style={agendaStyles.controls}>
            <View style={agendaSelectedDayStyles.container}>
                <SelectedDay/>
                <CalendarLegend/>
            </View>
            <CalendarActionsButtonGroup actions={this.props.actions}/>
        </View>;
    }

    //----------------------------------------------------------------------------
    private dialog(dialogType: EventDialogType): JSX.Element {
        return <View style={agendaStyles.dialog}>
            <EventDialog dialogType={dialogType}/>
        </View>;
    }
}

//----------------------------------------------------------------------------
const mapStateToProps = (state: AppState): AgendaProps => ({
    dialogType: state.calendar ? state.calendar.eventDialog.dialogType : undefined
});

export const Agenda = connect<AgendaProps, {}, AgendaOwnProps, AppState>(mapStateToProps)(AgendaImpl);
