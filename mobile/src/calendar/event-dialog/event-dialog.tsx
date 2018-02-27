import React, { Component } from 'react';
import { View, TouchableOpacity, StyleSheet } from 'react-native';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { ApplicationIcon } from '../../override/application-icon';
import { StyledText } from '../../override/styled-text';
import { CalendarActions, cancelDialog } from '../../reducers/calendar/calendar.action';

import { layout, content, buttons } from './styles';
import { CalendarActionButton } from '../calendar-action-button';

export interface ButtonProps {
    label: string;
    action: () => CalendarActions;
}

export interface EventDialogProps {
    title: string;
    text: string;
    icon: string;
    cancel?: ButtonProps;
    accept?: ButtonProps;
}

interface EventDialogDispatchProps {
    dispatch: Dispatch<CalendarActions>;
}

export class EventDialogImpl extends Component<EventDialogProps & EventDialogDispatchProps> {
    public render() {
        const actionButtons = this.getActionButtons();

        return (
            <View style={layout.container}>
                <View style={layout.icon}>
                    <ApplicationIcon name={this.props.icon} style={content.icon} />
                </View>
                <View style={layout.content}>
                    <View style={layout.text}>
                        <StyledText style={content.title}>{this.props.title}</StyledText>
                        <View>
                            <StyledText style={content.text}>{this.props.text}</StyledText>
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
        if (!this.props.cancel || !this.props.accept) {
            return null;
        }

        return (
            <View style={layout.buttons}>
                <CalendarActionButton title={this.props.cancel.label} onPress={this.cancel} style={buttons.cancel} textStyle={buttons.cancelLabel} />
                <CalendarActionButton title={this.props.accept.label} onPress={this.accept} style={buttons.accept} textStyle={buttons.acceptLabel} />
            </View>
        );
    }

    private close = () => {
        if (this.props.cancel) {
            this.cancel();
            return;
        }
        this.props.dispatch(cancelDialog());
    }

    private cancel = () => {
        this.props.dispatch(this.props.cancel.action());
    }

    private accept = () => {
        this.props.dispatch(this.props.accept.action());
    }
}

const mapStateToProps = (state: AppState) => ({
    ...state.calendar.calendarEvents.dialog as EventDialogProps
});

const mapDispatchToProps = (dispatch: Dispatch<CalendarActions>) => ({
    dispatch: dispatch
});

export const EventDialog = connect(mapStateToProps, mapDispatchToProps)(EventDialogImpl);