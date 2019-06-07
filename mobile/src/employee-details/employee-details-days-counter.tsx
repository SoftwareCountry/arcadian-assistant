/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { Employee } from '../reducers/organization/employee.model';
import { View } from 'react-native';
import { ApplicationIcon } from '../override/application-icon';
import { StyledText } from '../override/styled-text';
import { employeeDetailsDaysCounterStyles } from './styles';

//============================================================================
export interface DaysCounterData {
    icon: string;
    iconSize: number;
    title: string;
    text: string;
}

//============================================================================
interface EmployeeDetailsDaysCounterOwnProps {
    data: DaysCounterData;
}

export type EmployeeDetailsDaysCounterProps = EmployeeDetailsDaysCounterOwnProps;

//============================================================================
export class EmployeeDetailsDaysCounter extends Component<EmployeeDetailsDaysCounterProps> {
    //----------------------------------------------------------------------------
    public render() {
        const data = this.props.data;

        return (
            <View key={data.title} style={employeeDetailsDaysCounterStyles.container}>

                <View style={employeeDetailsDaysCounterStyles.iconContainer}>
                    <ApplicationIcon name={data.icon} size={data.iconSize} style={employeeDetailsDaysCounterStyles.icon}/>
                </View>

                <View style={employeeDetailsDaysCounterStyles.textContainer}>
                    <StyledText style={employeeDetailsDaysCounterStyles.title}>{data.title}</StyledText>
                    <StyledText style={employeeDetailsDaysCounterStyles.text}>{data.text}</StyledText>
                </View>

            </View>
        );
    }
}
