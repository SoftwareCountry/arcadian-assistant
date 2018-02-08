import React, { Component } from 'react';
import { View, Text } from 'react-native';
import { daysCounterStyles } from '../styles';
import { ArcadiaText } from '../../override/acradia-text';

interface DaysCounterProps {
    textValue: string;
    title: string[];
}

export class DaysCounter extends Component<DaysCounterProps> {

    public render() {
        return (
            <View style={daysCounterStyles.container}>
                <View style={daysCounterStyles.content}>
                    <ArcadiaText style={daysCounterStyles.contentValue}>{this.props.textValue}</ArcadiaText>
                    {
                        this.props.title
                            ? this.props.title.map((x, index) => <ArcadiaText key={index} style={daysCounterStyles.contentTitle}>{x}</ArcadiaText>)
                            : null
                    }
                </View>
            </View>
        );
    }
}

export const EmptyDaysCounter = () => (<View style={daysCounterStyles.container}></View>);