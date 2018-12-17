import React, { Component } from 'react';
import { LayoutChangeEvent, StyleSheet, View } from 'react-native';
import { chevronStyles } from './styles';

interface ChevronProps {
    containerWidth?: number;
}

interface ChevronState {
    chevronWidth: number;
}

export class Chevron extends Component<ChevronProps, ChevronState> {
    constructor(props: any) {
        super(props);
        this.state = {
            chevronWidth: 0
        };
    }

    public render() {
        const borderWidth = this.state.chevronWidth / 2;
        const rectangleStyles = StyleSheet.flatten([
            chevronStyles.chevron,
            {
                borderLeftWidth: borderWidth,
                borderRightWidth: borderWidth
            }
        ]);

        return (
            <View style={chevronStyles.container} onLayout={this.onLayout}>
                <View style={rectangleStyles}></View>
            </View>
        );
    }

    private onLayout = (e: LayoutChangeEvent) => {
        this.setState({
            chevronWidth: e.nativeEvent.layout.width as any
        });
    };
}

