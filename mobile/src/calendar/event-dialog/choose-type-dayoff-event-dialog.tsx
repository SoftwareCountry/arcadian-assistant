import React, { Component } from 'react';
import { EventDialogBase } from './event-dialog-base';
import { AppState } from '../../reducers/app.reducer';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import {
    closeEventDialog,
    EventDialogActions,
    openEventDialog
} from '../../reducers/calendar/event-dialog/event-dialog.action';
import { chosenTypeDayoff } from '../../reducers/calendar/dayoff.action';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { HoursCreditType } from '../../reducers/calendar/days-counters.model';
import { View } from 'react-native';
import { dayOffDialogStyles } from './styles';
import { RadioButton, RadioGroup } from '../../common/radio-buttons-group.component';
import { Optional } from 'types';

interface ChooseTypeDayoffEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmChosenType: (isWorkout: boolean) => void;
    closeDialog: () => void;
}

interface ChooseTypeDayoffEventDialogProps {
    chosenType: Optional<HoursCreditType>;
}

interface ChooseTypeDayoffEventDialogState {
    selectedHoursCreditType: Optional<HoursCreditType>;
    radioButtons: RadioButton[];
}

class ChooseTypeDayoffEventDialogImpl extends Component<ChooseTypeDayoffEventDialogProps & ChooseTypeDayoffEventDialogDispatchProps, ChooseTypeDayoffEventDialogState> {
    private readonly hoursCreditTypeToText = {
        [HoursCreditType.DaysOff]: 'dayoff',
        [HoursCreditType.Workout]: 'workout'
    };

    constructor(props: ChooseTypeDayoffEventDialogProps & ChooseTypeDayoffEventDialogDispatchProps) {
        super(props);
        this.state = {
            selectedHoursCreditType: props.chosenType,
            radioButtons: [
                {
                    selectionIndex: 0,
                    label: this.hoursCreditTypeToText[HoursCreditType.DaysOff],
                    selected: props.chosenType === HoursCreditType.DaysOff,
                    color: 'white',
                    labelStyle: dayOffDialogStyles.labelStyle,
                },
                {
                    selectionIndex: 1,
                    label: this.hoursCreditTypeToText[HoursCreditType.Workout],
                    selected: props.chosenType === HoursCreditType.Workout,
                    color: 'white',
                    labelStyle: dayOffDialogStyles.labelStyle,
                }
            ],
        };
    }

    public render() {
        return (
            <EventDialogBase
                title={'Select type to process your dayoff'}
                text={''}
                icon={'dayoff'}
                cancelLabel={'Back'}
                acceptLabel={'Confirm'}
                onAcceptPress={this.onAcceptClick}
                onCancelPress={this.onCancelClick}
                onClosePress={this.onCloseClick}>
                <View style={dayOffDialogStyles.container}>
                    <RadioGroup flexDirection={'row'}
                                radioButtons={this.state.radioButtons}
                                onPress={this.onDayoffTypeSelected}/>
                </View>
            </EventDialogBase>
        );
    }

    private onDayoffTypeSelected = (buttons: RadioButton[]) => {
        const selectedIndex = buttons.find((item => !!item.selected))!.selectionIndex;
        const selectedType = selectedIndex === 0 ? HoursCreditType.DaysOff : HoursCreditType.Workout;
        this.setState({
            selectedHoursCreditType: selectedType,
            radioButtons: buttons,
        });
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
}

const mapStateToProps = (state: AppState): ChooseTypeDayoffEventDialogProps => ({
    chosenType: state.calendar ? state.calendar.eventDialog.chosenHoursCreditType : undefined
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ChooseTypeDayoffEventDialogDispatchProps => ({
    cancelDialog: () => {
        dispatch(openEventDialog(EventDialogType.ProcessDayoff));
    },
    confirmChosenType: (isWorkout: boolean) => {
        dispatch(chosenTypeDayoff(isWorkout));
        dispatch(openEventDialog(EventDialogType.ConfirmDayoffStartDate));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

export const ChooseTypeDayoffEventDialog = connect(mapStateToProps, mapDispatchToProps)(ChooseTypeDayoffEventDialogImpl);
