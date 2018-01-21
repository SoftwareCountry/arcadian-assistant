import React, { Component } from 'react';
import { View, Text, StyleSheet, ProgressBarAndroid, Animated, ViewStyle, StyleProp, RegisteredStyle, Button } from 'react-native';
import { styles, colors } from '../../styles';
import { TileIndicator } from './tile-indicator';

interface TileDefaultProps {
    showAllDays?: boolean;
}

interface TileProps extends TileDefaultProps {
    leftDays: number;
    leftColor: string;
    allDays: number;
    allColor: string;
    title: string;
}

export class Tile extends Component<TileProps> {
    public static readonly defaultProps: TileDefaultProps = {
        showAllDays: true
    };

    public render() {

        const indicatorValue = this.props.allDays 
            ? (this.props.leftDays / this.props.allDays) * 100
            : 0;

        return (
            <View style={styles.tile}>
                <View style={styles.tileContent}>
                    <Text>
                        <Text style={styles.tileLeftDays}>{this.props.leftDays}</Text>
                        { this.props.showAllDays ? <Text style={styles.tileAllDays}> / {this.props.allDays}</Text> : null }
                    </Text>
                    <Text style={styles.tileTitle}>{this.props.title}</Text>
                </View>
                <TileIndicator value={indicatorValue} leftColor={this.props.leftColor} allColor={this.props.allColor} />
            </View>
        );
    }
}

export const EmptyTile = () => (<View style={styles.tile}></View>);