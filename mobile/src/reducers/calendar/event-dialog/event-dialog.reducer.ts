import { EventDialogActions } from './event-dialog.action';
import { EventDialogType } from './event-dialog-type.model';
import { HoursCreditType } from '../days-counters.model';
import { chosenTypeDayoff } from '../dayoff.action';

export interface EventDialogState {
    dialogType: EventDialogType;
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
        case 'START-PROGRESS':
            return {
                ...state,
                inProgress: true
            };
        case 'STOP-PROGRESS':
            return {
                ...state,
                inProgress: false
            };
        default:
            return state;
    }
};