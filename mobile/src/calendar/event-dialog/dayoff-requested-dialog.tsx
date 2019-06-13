/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { EventDialogBase } from './event-dialog-base';
import { connect } from 'react-redux';
import { Action, Dispatch } from 'redux';
import { closeEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';

//============================================================================
interface DayOffRequestedDialogDispatchProps {
    closeDialog: () => void;
}

//============================================================================
class DayOffRequestedDialogImpl extends Component<DayOffRequestedDialogDispatchProps> {
    public render() {
        return <EventDialogBase
            title={'Day off requested'}
            text={'Your day off request will be shortly reviewed by your supervisor'}
            icon={'day_off'}
            acceptLabel={'Ok'}
            onAcceptPress={this.props.closeDialog}
            onClosePress={this.props.closeDialog}/>;
    }
}

//----------------------------------------------------------------------------
const mapDispatchToProps = (dispatch: Dispatch<Action>): DayOffRequestedDialogDispatchProps => ({
    closeDialog: () => {
        dispatch(closeEventDialog());
    },
});

export const DayOffRequestedDialog = connect(undefined, mapDispatchToProps)(DayOffRequestedDialogImpl);
