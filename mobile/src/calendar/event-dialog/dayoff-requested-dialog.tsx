/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Component } from 'react';
import { EventDialogBase } from './event-dialog-base';
import React from 'react';
import { connect } from 'react-redux';
import { Action, Dispatch } from 'redux';
import { closeEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';

//============================================================================
interface DayoffRequestedDialogDispatchProps {
    closeDialog: () => void;
}

//============================================================================
class DayoffRequestedDialogImpl extends Component<DayoffRequestedDialogDispatchProps> {
    public render() {
        return <EventDialogBase
            title={'Dayoff requested'}
            text={'Your dayoff request will be shortly reviewed by your supervisor'}
            icon={'dayoff'}
            acceptLabel={'Ok'}
            onAcceptPress={this.props.closeDialog}
            onClosePress={this.props.closeDialog} />;
    }
}

//----------------------------------------------------------------------------
const mapDispatchToProps = (dispatch: Dispatch<Action>): DayoffRequestedDialogDispatchProps => ({
    closeDialog: () => {
        dispatch(closeEventDialog());
    },
});

const DayoffRequestedDialog = connect(undefined, mapDispatchToProps)(DayoffRequestedDialogImpl);

export default DayoffRequestedDialog;
