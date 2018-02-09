import React, { Component } from 'react';
import { TextProperties, StyleSheet, Text, ViewStyle, TextStyle } from 'react-native';

const commonStyles = StyleSheet.create({
    text: {
        fontFamily: 'CenturyGothic'
    }
});

export class StyledText extends Component<TextProperties> {
    public render() {
        const textStyles = StyleSheet.flatten([
            this.props.style,
            commonStyles.text
        ]);

        return <Text {...this.props} style={textStyles}>{this.props.children}</Text>;
    }
}