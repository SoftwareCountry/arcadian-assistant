/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { Alert, TouchableOpacity, View } from 'react-native';
import { ApplicationIcon } from '../override/application-icon';
import { layoutStylesForEventManagementToolset } from './styles';
import { EventActionsContainer } from './event-action-provider';

//============================================================================
interface EventManagementToolsetProps {
    eventAction: EventActionsContainer;
}

//============================================================================
export class EventManagementToolset extends Component<EventManagementToolsetProps> {
    //----------------------------------------------------------------------------
    public onApprove = () => {
        const { positiveAction } = this.props.eventAction;

        if (!positiveAction) {
            console.warn('Positive action is not set while onApprove is called');
            return;
        }

        Alert.alert(
            `Are you sure you want to ${positiveAction.name} the request?`,
            undefined,
            [
                {
                    text: 'Cancel',
                    style: 'cancel',
                },
                {
                    text: this.capitalizeFirstLetter(`${positiveAction.name}`),
                    onPress: positiveAction.handler,
                },
            ],
        );
    };

    //----------------------------------------------------------------------------
    public onReject = () => {
        const { negativeAction } = this.props.eventAction;

        if (!negativeAction) {
            console.warn('Negative action is not set while onReject is called');
            return;
        }

        Alert.alert(
            `Are you sure you want to ${negativeAction.name} the request?`,
            undefined,
            [
                {
                    text: 'Cancel',
                    style: 'cancel',
                },
                {
                    text: this.capitalizeFirstLetter(`${negativeAction.name}`),
                    onPress: negativeAction.handler,
                },
            ],
        );
    };

    //----------------------------------------------------------------------------
    public render() {
        const { toolsetContainer, approveIcon, rejectIcon } = layoutStylesForEventManagementToolset;

        const canApprove = !!this.props.eventAction.positiveAction;
        const canReject = !!this.props.eventAction.negativeAction;

        return (
            <View style={toolsetContainer}>
                {
                    canApprove ?
                    <TouchableOpacity onPress={this.onApprove}>
                        <ApplicationIcon name={'approve-tick'} style={approveIcon}/>
                    </TouchableOpacity> :
                    <View/>
                }
                {
                    canReject ?
                    <TouchableOpacity onPress={this.onReject}>
                        <ApplicationIcon name={'reject-cross'} style={rejectIcon}/>
                    </TouchableOpacity> :
                    <View/>
                }
            </View>
        );
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private capitalizeFirstLetter(str: string) {
        return str.charAt(0).toLocaleUpperCase() + str.slice(1);
    }
}
