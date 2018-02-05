import React, { Component } from 'react';
import { View, Text } from 'react-native';
import { daysCounterStyles } from '../styles';

interface DaysCounterProps {
    textValue: string;
    title: string[];
}

export class DaysCounter extends Component<DaysCounterProps> {

    public render() {
        return (
            <View style={daysCounterStyles.container}>
                <View style={daysCounterStyles.content}>
                    <Text style={daysCounterStyles.contentValue}>{this.props.textValue}</Text>
                    {
                        this.props.title
                            ? this.props.title.map((x, index) => <Text key={index} style={daysCounterStyles.contentTitle}>{x}</Text>)
                            : null
                    }
                </View>
            </View>
        );
    }
}

export const EmptyDaysCounter = () => (<View style={daysCounterStyles.container}></View>);