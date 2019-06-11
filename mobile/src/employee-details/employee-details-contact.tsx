/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { TouchableOpacity, View } from 'react-native';
import { Nullable } from 'types';
import React, { Component } from 'react';
import { ApplicationIcon } from '../override/application-icon';
import { StyledText } from '../override/styled-text';
import { employeeDetailsContactStyles } from './styles';

//============================================================================
export interface ContactData {
    icon: string;
    text: Nullable<string>;
    title: string;
    size: number;
    prefix: string;
}

//============================================================================
interface EmployeeDetailsContactOwnProps {
    data: ContactData;
    onPress: Nullable<() => void>;
}

export type EmployeeDetailsContactProps = EmployeeDetailsContactOwnProps;

//============================================================================
export class EmployeeDetailsContact extends Component<EmployeeDetailsContactProps> {
    //----------------------------------------------------------------------------
    public render() {
        const contact = this.props.data;

        return (
            <TouchableOpacity onPress={this.props.onPress ? this.props.onPress : undefined}>

                <View style={employeeDetailsContactStyles.container}>

                    <View style={employeeDetailsContactStyles.iconContainer}>
                        <ApplicationIcon name={contact.icon} size={contact.size} style={employeeDetailsContactStyles.icon}/>
                    </View>

                    <View style={employeeDetailsContactStyles.textContainer}>
                        <StyledText style={employeeDetailsContactStyles.title}>{contact.title}</StyledText>
                        <StyledText style={employeeDetailsContactStyles.text}>{contact.text}</StyledText>
                    </View>

                </View>

            </TouchableOpacity>
        );
    }
}
