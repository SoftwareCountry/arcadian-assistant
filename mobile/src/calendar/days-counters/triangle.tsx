import React, { Component } from 'react';
import { View, StyleSheet } from 'react-native';
import { triangleStyles } from '../styles';

interface TriangleProps {
    containerWidth: number;
}

export class Triangle extends Component<TriangleProps> {
    public render() {
        const borderWidth = this.props.containerWidth / 2;
        const styles = StyleSheet.flatten([
            triangleStyles.container,
            {
                borderLeftWidth: borderWidth,
                borderRightWidth: borderWidth,
            }
        ]);

        return <View style={styles}></View>;
    }
}

