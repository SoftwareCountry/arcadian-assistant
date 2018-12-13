/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { Alert, TouchableOpacity, View } from 'react-native';
import { ApplicationIcon } from '../override/application-icon';
import { layoutStylesForEventManagementToolset } from './styles';
import { EventAction, EventActionContainer, EventActionType } from './event-action-provider';
import { capitalizeFirstLetter } from '../utils/string';

//============================================================================
interface EventManagementToolsetProps {
    eventAction: EventActionContainer;
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

        const verb = `${this.actionVerb(positiveAction)}`;

        Alert.alert(
            `Are you sure you want to ${verb} the request?`,
            undefined,
            [
                {
                    text: 'Cancel',
                    style: 'cancel',
                },
                {
                    text: capitalizeFirstLetter(`${verb}`),
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

        const verb = `${this.actionVerb(negativeAction)}`;

        Alert.alert(
            `Are you sure you want to ${verb} the request?`,
            undefined,
            [
                {
                    text: 'Cancel',
                    style: 'cancel',
                },
                {
                    text: capitalizeFirstLetter(`${verb}`),
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
    private actionVerb(action: EventAction): string {
        switch (action.type) {
            case EventActionType.approve:
                return 'approve';

            case EventActionType.reject:
                return 'reject';

            case EventActionType.cancel:
                return 'discard';
        }
    }
}
