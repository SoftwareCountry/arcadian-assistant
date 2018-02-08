import React, { Component } from 'react';
import { View, StyleSheet, LayoutChangeEvent } from 'react-native';
import { layoutStyles } from './styles';

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
            layoutStyles.chevron,
            {
                borderLeftWidth: borderWidth,
                borderRightWidth: borderWidth,
            }
        ]);

        return <View style={rectangleStyles} onLayout={this.onDaysCountersLayout}></View>;
    }

    private onDaysCountersLayout = (e: LayoutChangeEvent) => {
        this.setState({
            chevronWidth: e.nativeEvent.layout.width as any
        });
    }
}

