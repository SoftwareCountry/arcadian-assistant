import React, { Component } from 'react';
import { TextProperties, StyleSheet, Text } from 'react-native';

export class ArcadiaText extends Component<TextProperties> {
    public render() {
        const textStyles = StyleSheet.flatten([
            this.props.style,
            {
                fontFamily: 'CenturyGothic'
            }
        ]);

        return <Text {...this.props} style={textStyles}>{this.props.children}</Text>;
    }
}