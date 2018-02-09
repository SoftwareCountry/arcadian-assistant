import React, { Component } from 'react';
import { View } from 'react-native';
import { daysCounterStyles } from '../styles';
import { StyledText } from '../../override/styled-text';

interface DaysCounterProps {
    textValue: string;
    title: string[];
}

export class DaysCounter extends Component<DaysCounterProps> {

    public render() {
        return (
            <View style={daysCounterStyles.container}>
                <View style={daysCounterStyles.content}>
                    <StyledText style={daysCounterStyles.contentValue}>{this.props.textValue}</StyledText>
                    {
                        this.props.title
                            ? this.props.title.map((x, index) => <StyledText key={index} style={daysCounterStyles.contentTitle}>{x}</StyledText>)
                            : null
                    }
                </View>
            </View>
        );
    }
}

export const EmptyDaysCounter = () => (<View style={daysCounterStyles.container}></View>);