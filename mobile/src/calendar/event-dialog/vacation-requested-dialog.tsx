/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import { EventDialogBase } from './event-dialog-base';
import { connect } from 'react-redux';
import { Action, Dispatch } from 'redux';
import { closeEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';

//============================================================================
interface VacationRequestedDialogDispatchProps {
    closeDialog: () => void;
}

//============================================================================
class VacationRequestedDialogImpl extends Component<VacationRequestedDialogDispatchProps> {
    public render() {
        return <EventDialogBase
            title={'Vacation requested'}
            text={'You will receive a notification when your vacation is approved and when your documents are ready to sign'}
            icon={'vacation'}
            acceptLabel={'Ok'}
            onAcceptPress={this.props.closeDialog}
            onClosePress={this.props.closeDialog}/>;
    }
}

//----------------------------------------------------------------------------
const mapDispatchToProps = (dispatch: Dispatch<Action>): VacationRequestedDialogDispatchProps => ({
    closeDialog: () => {
        dispatch(closeEventDialog());
    },
});

export const VacationRequestedDialog = connect(undefined, mapDispatchToProps)(VacationRequestedDialogImpl);
