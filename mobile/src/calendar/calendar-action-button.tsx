import React, { Component } from 'react';
import { LayoutChangeEvent, StyleSheet, TextStyle, TouchableOpacity, View, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { calendarActionsStyles } from './styles';

interface CalendarActionButtonProps {
    onPress: () => void;
    borderColor?: string;
    title: string;
    style?: ViewStyle;
    textStyle?: TextStyle;
    disabled?: boolean;
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
            this.state.buttonHeight === 0 // Prevent border flicker
                ? { borderWidth: 0, borderColor: 'transparent' } : {},
            this.props.style
        ]);

        const buttonContainerStyles = StyleSheet.flatten([
            calendarActionsStyles.buttonContainer,
            this.props.disabled
                ? { opacity: 0.1 }
                : null
        ]);

        const textStyle = StyleSheet.flatten([
            calendarActionsStyles.buttonTitle,
            this.props.textStyle
        ]);

        return (
            <View style={buttonContainerStyles} onLayout={this.onButtonContainerLayout}>
                <TouchableOpacity style={buttonStyles} onPress={this.onButtonEditorPress}
                                  disabled={this.props.disabled}>
                    <StyledText style={textStyle}>{this.props.title}</StyledText>
                </TouchableOpacity>
            </View>
        );
    }

    private onButtonContainerLayout = (e: LayoutChangeEvent) => {
        this.setState({
            buttonHeight: e.nativeEvent.layout.height
        });
    };

    private onButtonEditorPress = () => {
        this.props.onPress();
    };
}

export const CalendarActionButtonSeparator = () => <View style={calendarActionsStyles.separator}/>;
