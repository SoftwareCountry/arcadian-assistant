import React, { Component } from 'react';
import { View } from 'react-native';
import { daysCounterStyles } from './styles';
import { StyledText } from '../../override/styled-text';
import { ApplicationIcon } from '../../override/application-icon';
import { LoadingView } from '../../navigation/loading';

interface DaysCounterProps {
    textValue: string;
    title: string[];
    icon?: {
        name: string,
        size: number
    };
}

export class DaysCounter extends Component<DaysCounterProps> {

    public render() {
        const icon = this.props.icon
            ? <ApplicationIcon name={this.props.icon.name} size={this.props.icon.size} style={daysCounterStyles.icon}/>
            : null;

        const title = this.props.title
            ? <StyledText style={daysCounterStyles.label}>{this.props.title.join(' ')}</StyledText>
            : null;

        return (
            <View style={daysCounterStyles.container}>
                <View style={daysCounterStyles.content}>
                    <View style={daysCounterStyles.counterContainer}>
                        {icon}
                        <StyledText style={daysCounterStyles.counter}>{this.props.textValue}</StyledText>
                    </View>
                    {title}
                </View>
            </View>
        );
    }
}

export const EmptyDaysCounter = () => (
    <View style={daysCounterStyles.container}>
        <LoadingView/>
    </View>
);
