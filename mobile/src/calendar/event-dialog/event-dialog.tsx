import React, { Component } from 'react';
import { View, TouchableOpacity, StyleSheet } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { ApplicationIcon } from '../../override/application-icon';
import { StyledText } from '../../override/styled-text';
import { CalendarActions, cancelDialog } from '../../reducers/calendar/calendar.action';

import { layout, content, buttons } from './styles';
import { CalendarActionButton } from '../calendar-action-button';
import { EventDialogModel, EventDialogEmptyModel } from '../../reducers/calendar/event-dialog/event-dialog.model';
import { Employee } from '../../reducers/organization/employee.model';
import { CalendarEvents } from '../../reducers/calendar/calendar-events.model';

interface EventDialogProps {
    model?: EventDialogModel<any>;
}

interface EventDialogDispatchProps {
    dispatch: Dispatch<CalendarActions>;
}

export class EventDialogImpl extends Component<EventDialogProps & EventDialogDispatchProps> {
    public render() {
        const actionButtons = this.getActionButtons();
        const model = this.props.model ? this.props.model : new EventDialogEmptyModel();

        return (
            <View style={layout.container}>
                <View style={layout.icon}>
                    <ApplicationIcon name={model.icon} style={content.icon} />
                </View>
                <View style={layout.content}>
                    <View style={layout.text}>
                        <StyledText style={content.title}>{model.title}</StyledText>
                        <View>
                            <StyledText style={content.text}>{model.text}</StyledText>
                        </View>
                    </View>

                    {actionButtons}
                </View>
                <View style={layout.close}>
                    <TouchableOpacity onPress={this.close}>
                        <StyledText style={buttons.close}>˟</StyledText>
                    </TouchableOpacity>
                </View>
            </View>
        );
    }

    private getActionButtons() {
        const { model } = this.props;

        if (!model || !model.cancelLabel || !model.acceptLabel) {
            return null;
        }

        return (
            <View style={layout.buttons}>
                <CalendarActionButton title={model.cancelLabel} onPress={this.cancel} style={buttons.cancel} textStyle={buttons.cancelLabel} />
                <CalendarActionButton title={model.acceptLabel} onPress={this.accept} style={buttons.accept} textStyle={buttons.acceptLabel} />
            </View>
        );
    }

    private close = () => {
        if (!this.props.model.close) {
            this.cancel();
            return;
        }
        this.props.dispatch(this.props.model.close());
    }

    private cancel = () => {
        this.props.dispatch(this.props.model.cancelAction());
    }

    private accept = () => {
        this.props.dispatch(this.props.model.acceptAction());
    }
}

const mapStateToProps = (state: AppState): EventDialogProps => ({
    model: state.calendar.calendarEvents.dialog.model
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>): EventDialogDispatchProps => ({
    dispatch: dispatch
});

export const EventDialog = connect(mapStateToProps, mapDispatchToProps)(EventDialogImpl);