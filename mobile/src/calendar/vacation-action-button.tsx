import React, { Component } from 'react';
import { CalendarActionButton } from './calendar-action-button';
import { IntervalModel } from '../reducers/calendar/calendar.model';
import { CalendarEventsColor } from './styles';
import { Approval } from '../reducers/calendar/approval.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Set } from 'immutable';

//============================================================================
interface VacationActionButtonStateProps {
    approvals: Set<Approval>;
}

//============================================================================
interface VacationActionButtonOwnProps {
    interval?: IntervalModel;
    disabled: boolean;
    request: () => void;
    edit: () => void;
}

//============================================================================
class VacationActionButtonImpl extends Component<VacationActionButtonOwnProps & VacationActionButtonStateProps> {

    //----------------------------------------------------------------------------
    public render() {
        const disableActionButton = this.disableCalendarAction();

        return (
            <CalendarActionButton
                title={this.title}
                borderColor={CalendarEventsColor.vacation}
                onPress={this.onVacationAction}
                disabled={this.props.disabled || disableActionButton}/>
        );
    }

    //----------------------------------------------------------------------------
    public get title(): string {
        return !this.props.interval
            ? 'Request Vacation'
            : 'Edit Vacation';
    }

    //----------------------------------------------------------------------------
    public onVacationAction = () => {
        if (!this.props.interval) {
            this.props.request();
        } else {
            this.props.edit();
        }
    };

    //----------------------------------------------------------------------------
    private disableCalendarAction(): boolean {
        const { interval } = this.props;

        return !!interval &&
            (interval.calendarEvent.isCompleted ||
                interval.calendarEvent.isApproved ||
                interval.calendarEvent.isAccountingReady ||
                interval.calendarEvent.isProcessed ||
                this.isPartiallyApproved());
    }

    //----------------------------------------------------------------------------
    private isPartiallyApproved(): boolean {
        const { interval, approvals } = this.props;

        if (!interval || !approvals) {
            return false;
        }

        return approvals.size === 1;
    }
}

//----------------------------------------------------------------------------
function getApprovals(state: AppState, ownProps: VacationActionButtonOwnProps): Set<Approval> {
    const { calendar } = state;
    const { interval } = ownProps;

    if (!calendar || !interval) {
        return Set<Approval>();
    }

    return calendar.calendarEvents.approvals.get(interval.calendarEvent.calendarEventId, Set<Approval>());
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState, ownProps: VacationActionButtonOwnProps): VacationActionButtonStateProps => ({
    approvals: getApprovals(state, ownProps),
});

//----------------------------------------------------------------------------
export const VacationActionButton = connect(stateToProps)(VacationActionButtonImpl);

