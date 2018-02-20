import React, { Component } from 'react';
import { TouchableOpacity, ViewStyle, LayoutChangeEvent, StyleSheet, View } from 'react-native';
import { StyledText } from '../override/styled-text';
import { eventsEditorStyles } from './styles';

export type OnPressEventsEditorButton = (isRequest: boolean) => void;

interface EventsEditorButtonProps {
    requestTitle: string;
    reviewTitle: string;
    onPress: OnPressEventsEditorButton;
    borderColor: string;
    isRequest: boolean;
}

interface EventsEditorButtonState {
    buttonHeight: number;
}

export class EventsEditorButton extends Component<EventsEditorButtonProps, EventsEditorButtonState> {
    constructor(props: EventsEditorButtonProps) {
        super(props);
        this.state = {
            buttonHeight: 0
        };
    }

    public render() {

        const buttonStyles = StyleSheet.flatten([
            eventsEditorStyles.button,
            {
                height: this.state.buttonHeight,
                borderColor: this.props.borderColor,
                borderRadius: this.state.buttonHeight
            }
        ]);

        return (
            <TouchableOpacity style={buttonStyles} onLayout={this.onButtonLayout} onPress={this.onButtonEditorPress}>
                <StyledText style={eventsEditorStyles.buttonTitle}>{this.title}</StyledText>
            </TouchableOpacity>
        );
    }

    private onButtonLayout = (e: LayoutChangeEvent) => {
        this.setState({
            buttonHeight: e.nativeEvent.layout.height
        });
    }

    private get title(): string {
        return this.props.isRequest 
            ? this.props.requestTitle 
            : this.props.reviewTitle;
    }

    private onButtonEditorPress = () => {
        this.props.onPress(this.props.isRequest);
    }
}

export const EventsEditorSeparator = () => <View style={{flex: 1}}></View>;