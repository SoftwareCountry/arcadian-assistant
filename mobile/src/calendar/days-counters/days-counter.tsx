import React, { Component } from 'react';
import { View } from 'react-native';
import { StyleDays } from './styles';
import { StyledText } from '../../override/styled-text';
import { ApplicationIcon } from '../../override/application-icon';
import { LoadingView } from '../../navigation/loading';

//============================================================================
interface DaysCounterProps {
    textValue: string;
    title: string[];
    icon?: {
        name: string,
        size: number
    };
}

//============================================================================
export class DaysCounter extends Component<DaysCounterProps> {
    //----------------------------------------------------------------------------
    public render() {
        const icon = this.props.icon
            ? <ApplicationIcon name={this.props.icon.name} size={this.props.icon.size} style={StyleDays.counter.icon}/>
            : null;

        const title = this.props.title
            ? <StyledText style={StyleDays.counter.label}>{this.props.title.join(' ')}</StyledText>
            : null;

        return (
            <View style={StyleDays.counter.container}>
                <View style={StyleDays.counter.content}>
                    <View style={StyleDays.counter.counterContainer}>
                        {icon}
                        <StyledText style={StyleDays.counter.counter}>{this.props.textValue}</StyledText>
                    </View>
                    {title}
                </View>
            </View>
        );
    }
}

//----------------------------------------------------------------------------
export const EmptyDaysCounter = () => (
    <View style={StyleDays.counter.container}>
        <LoadingView/>
    </View>
);
