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
import { chosenTypeDayOff } from '../../reducers/calendar/dayoff.action';
import { EventDialogType } from '../../reducers/calendar/event-dialog/event-dialog-type.model';
import { HoursCreditType } from '../../reducers/calendar/days-counters.model';
import { View } from 'react-native';
import { dayOffDialogStyles } from './styles';
import { RadioButton, RadioGroup } from '../../common/radio-buttons-group.component';
import { Optional } from 'types';

interface ChooseTypeDayOffEventDialogDispatchProps {
    cancelDialog: () => void;
    confirmChosenType: (isWorkout: boolean) => void;
    closeDialog: () => void;
}

interface ChooseTypeDayOffEventDialogProps {
    chosenType: Optional<HoursCreditType>;
}

interface ChooseTypeDayOffEventDialogState {
    selectedHoursCreditType: Optional<HoursCreditType>;
    radioButtons: RadioButton[];
}

class ChooseTypeDayOffEventDialogImpl extends Component<ChooseTypeDayOffEventDialogProps & ChooseTypeDayOffEventDialogDispatchProps, ChooseTypeDayOffEventDialogState> {
    private readonly hoursCreditTypeToText = {
        [HoursCreditType.DaysOff]: 'day off',
        [HoursCreditType.Workout]: 'workout'
    };

    constructor(props: ChooseTypeDayOffEventDialogProps & ChooseTypeDayOffEventDialogDispatchProps) {
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
                title={'Select type to process your day off'}
                text={''}
                icon={'day_off'}
                cancelLabel={'Back'}
                acceptLabel={'Confirm'}
                onAcceptPress={this.onAcceptClick}
                onCancelPress={this.onCancelClick}
                onClosePress={this.onCloseClick}>
                <View style={dayOffDialogStyles.container}>
                    <RadioGroup flexDirection={'row'}
                                radioButtons={this.state.radioButtons}
                                onPress={this.onDayOffTypeSelected}/>
                </View>
            </EventDialogBase>
        );
    }

    private onDayOffTypeSelected = (buttons: RadioButton[]) => {
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

const mapStateToProps = (state: AppState): ChooseTypeDayOffEventDialogProps => ({
    chosenType: state.calendar ? state.calendar.eventDialog.chosenHoursCreditType : undefined
});

const mapDispatchToProps = (dispatch: Dispatch<EventDialogActions>): ChooseTypeDayOffEventDialogDispatchProps => ({
    cancelDialog: () => {
        dispatch(openEventDialog(EventDialogType.ProcessDayOff));
    },
    confirmChosenType: (isWorkout: boolean) => {
        dispatch(chosenTypeDayOff(isWorkout));
        dispatch(openEventDialog(EventDialogType.ConfirmDayOffStartDate));
    },
    closeDialog: () => {
        dispatch(closeEventDialog());
    }
});

export const ChooseTypeDayOffEventDialog = connect(mapStateToProps, mapDispatchToProps)(ChooseTypeDayOffEventDialogImpl);
