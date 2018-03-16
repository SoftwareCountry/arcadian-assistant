import React, { Component } from 'react';
import { layout, content, buttons } from './styles';
import { View, TouchableOpacity } from 'react-native';
import { ApplicationIcon } from '../../override/application-icon';
import { StyledText } from '../../override/styled-text';
import { CalendarActionButton } from '../calendar-action-button';

interface EventDialogBaseDefaultProps {
    disableAccept?: boolean;
    close?: () => void;
}

interface EventDialogBaseProps extends EventDialogBaseDefaultProps {
    icon: string;
    title: string;
    text: string;
    cancelLabel: string;
    acceptLabel: string;
    cancelAction: () => void;
    acceptAction: () => void;
}

export const eventDialogTextDateFormat = 'MMMM D, YYYY';

export class EventDialogBase extends Component<EventDialogBaseProps> {
    public static defaultProps: EventDialogBaseDefaultProps = {
        disableAccept: false
    };

    public render() {
        const actionButtons = this.getActionButtons();

        const { icon, title, text } = this.props;

        return (
            <View style={layout.container}>
                <View style={layout.icon}>
                    <ApplicationIcon name={this.props.icon} style={content.icon} />
                </View>
                <View style={layout.content}>
                    <View style={layout.text}>
                        <StyledText style={content.title}>{title}</StyledText>
                        <View>
                            <StyledText style={content.text}>{text}</StyledText>
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
        const { cancelLabel, acceptLabel, disableAccept } = this.props;

        return (
            <View style={layout.buttons}>
                <CalendarActionButton title={cancelLabel} onPress={this.cancel} style={buttons.cancel} textStyle={buttons.cancelLabel} />
                <CalendarActionButton title={acceptLabel} onPress={this.accept} style={buttons.accept} textStyle={buttons.acceptLabel} disabled={disableAccept} />
            </View>
        );
    }

    private close = () => {
        if (!this.props.close) {
            this.cancel();
            return;
        }
        this.props.close();
    }

    private cancel = () => {
        this.props.cancelAction();
    }

    private accept = () => {
        this.props.acceptAction();
    }
}