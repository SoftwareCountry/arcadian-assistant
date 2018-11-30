import { EventDialogActions } from './event-dialog.action';
import { EventDialogType } from './event-dialog-type.model';
import { HoursCreditType } from '../days-counters.model';
import { Optional } from 'types';

export interface EventDialogState {
    dialogType: Optional<EventDialogType>;
    chosenHoursCreditType: HoursCreditType;
    inProgress: boolean;
}

const initState: EventDialogState = {
    dialogType: null,
    chosenHoursCreditType: HoursCreditType.DaysOff,
    inProgress: false
};

export const eventDialogReducer = (state: EventDialogState = initState, action: EventDialogActions): EventDialogState => {
    switch (action.type) {
        case 'OPEN-EVENT-DIALOG':
            return {
                ...state,
                dialogType: action.dialogType
            };
        case 'CLOSE-EVENT-DIALOG': {
            return {
                ...state,
                dialogType: null,
                chosenHoursCreditType: HoursCreditType.DaysOff
            };
        }
        case 'CHOSEN-TYPE-DAYOFF': {
            return {
                ...state,
                chosenHoursCreditType: action.isWorkout ? HoursCreditType.Workout : HoursCreditType.DaysOff
            };
        }
        case 'START-EVENT-DIALOG-PROGRESS':
            return {
                ...state,
                inProgress: true
            };
        case 'STOP-EVENT-DIALOG-PROGRESS':
            return {
                ...state,
                inProgress: false
            };
        default:
            return state;
    }
};
