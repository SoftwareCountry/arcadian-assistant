import React, { Component } from 'react';
import { EventDialogBase, eventDialogTextDateFormat } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { EventDialogActions, closeEventDialog, openEventDialog } from '../../reducers/calendar/event-dialog/event-dialog.action';
import { chosenTypeDayoff } from '../../reducers/calendar/dayoff.action';
import { DayModel } from '../../reducers/calendar/calendar.model';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { HoursCreditType } from '../../reducers/calendar/days-counters.model';
import { SelectorDayoffType } from './selector-dayoff-type';

interface ChooseTypeDayoffEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmChosenType: (isWorkout: boolean) => void;
    closeDialog: () => void;
}

interface ChooseTypeDayoffEventDialogProps {
    chosenType : HoursCreditType;
}

interface ChooseTypeDayoffEventDialogState {
    selectedHoursCreditType: HoursCreditType;
}

class ChooseTypeDayoffEventDialogImpl extends Component<ChooseTypeDayoffEventDialogProps & ChooseTypeDayoffEventDialogDispatchProps, ChooseTypeDayoffEventDialogState> {
    private readonly hoursCreditTypeToText = {
        [HoursCreditType.DaysOff]: 'dayoff',
        [HoursCreditType.Workout]: 'workout'
    };

    constructor(props: ChooseTypeDayoffEventDialogProps & ChooseTypeDayoffEventDialogDispatchProps) {
        super(props);
        this.state = {
            selectedHoursCreditType: props.chosenType
        };
    }

    public render() {
        return <EventDialogBase
                    title={'Select type to process your dayoff'}
                    text={this.text}
                    icon={'dayoff'}
                    cancelLabel={'Back'}
                    acceptLabel={'Confirm'}
                    onAcceptPress={this.onAcceptClick}
                    onCancelPress={this.onCancelClick}
                    onClosePress={this.onCloseClick} >
                    <SelectorDayoffType onTypeSelected={this.onDayoffTypeSelected} />
                </EventDialogBase>;
    }

    private onDayoffTypeSelected = (selectedType: HoursCreditType) => {
        this.setState({ selectedHoursCreditType: selectedType });
    };

    private onCancelClick = () => {
        this.props.cancelDialog();
    };

    private onAcceptClick = () => {
        this.props.confirmChosenType(this.state.selectedHoursCreditType === HoursCreditType.Workout);
    };

    private onCloseClick = () => {
        this.props.closeDialog();
    };

    public get text(): string {
        const hoursCreditTypeToText = this.hoursCreditTypeToText[this.state.selectedHoursCreditType];

        return `Type is ${hoursCreditTypeToText}`;
    }
}

const mapStateToProps = (state: AppState): ChooseTypeDayoffEventDialogProps => ({
    chosenType: state.calendar.eventDialog.chosenHoursCreditType
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ChooseTypeDayoffEventDialogDispatchProps => ({
    cancelDialog: () => { dispatch(openEventDialog(EventDialogType.ProcessDayoff)); },
    confirmChosenType: (isWorkout: boolean) => {
        dispatch(chosenTypeDayoff(isWorkout));
        dispatch(openEventDialog(EventDialogType.ConfirmDayoffStartDate));
    },
    closeDialog: () => { dispatch(closeEventDialog()); }
});

export const ChooseTypeDayoffEventDialog = connect(mapStateToProps, mapDispatchToProps)(ChooseTypeDayoffEventDialogImpl);
