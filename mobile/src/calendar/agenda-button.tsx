import React, { Component } from 'react';
import { TouchableOpacity, ViewStyle, LayoutChangeEvent, StyleSheet, View } from 'react-native';
import { StyledText } from '../override/styled-text';
import { agendaPrimaryButtonsStyles } from './styles';

interface AgendaPrimaryButtonProps {
    title: string;
    borderColor: string;
}

interface AgendaPrimaryButtonState {
    buttonHeight: number;
}

export class AgendaPrimaryButton extends Component<AgendaPrimaryButtonProps, AgendaPrimaryButtonState> {
    constructor(props: AgendaPrimaryButtonProps) {
        super(props);
        this.state = {
            buttonHeight: 0
        };
    }

    public render() {
        const { title, borderColor } = this.props;

        const buttonStyles = StyleSheet.flatten([
            agendaPrimaryButtonsStyles.primaryButton,
            {
                height: this.state.buttonHeight,
                borderColor: borderColor,
                borderRadius: this.state.buttonHeight
            }
        ]);

        return (
            <TouchableOpacity style={buttonStyles} onLayout={this.onButtonLayout}>
                <StyledText style={agendaPrimaryButtonsStyles.primaryButtonTitle}>{title}</StyledText>
            </TouchableOpacity>
        );
    }

    private onButtonLayout = (e: LayoutChangeEvent) => {
        this.setState({
            buttonHeight: e.nativeEvent.layout.height
        });
    }
}

export const AgendaPrimarySeparator = () => <View style={{flex: 1}}></View>;