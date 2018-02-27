import React, { Component } from 'react';
import { TouchableOpacity, ViewStyle, LayoutChangeEvent, StyleSheet, View, TextStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { calendarActionsStyles } from './styles';

interface CalendarActionButtonProps {
    onPress: () => void;
    borderColor?: string;
    title: string;
    style?: ViewStyle;
    textStyle?: TextStyle;
}

interface CalendarActionButtonState {
    buttonHeight: number;
}

export class CalendarActionButton extends Component<CalendarActionButtonProps, CalendarActionButtonState> {
    constructor(props: CalendarActionButtonProps) {
        super(props);
        this.state = {
            buttonHeight: 0
        };
    }

    public render() {

        const buttonStyles = StyleSheet.flatten([
            calendarActionsStyles.button,
            {
                height: this.state.buttonHeight,
                borderColor: this.props.borderColor,
                borderRadius: this.state.buttonHeight
            },
            this.props.style
        ]);

        const textStyle = StyleSheet.flatten([
            calendarActionsStyles.buttonTitle,
            this.props.textStyle
        ]);

        return (
            <TouchableOpacity style={buttonStyles} onLayout={this.onButtonLayout} onPress={this.onButtonEditorPress}>
                <StyledText style={textStyle}>{this.props.title}</StyledText>
            </TouchableOpacity>
        );
    }

    private onButtonLayout = (e: LayoutChangeEvent) => {
        this.setState({
            buttonHeight: e.nativeEvent.layout.height
        });
    }

    private onButtonEditorPress = () => {
        this.props.onPress();
    }
}

export const CalendarActionButtonSeparator = () => <View style={calendarActionsStyles.separator}></View>;