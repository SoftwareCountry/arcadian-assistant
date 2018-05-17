import { EventDialogActions } from './event-dialog.action';
import { EventDialogType } from './event-dialog-type.model';
import { HoursCreditType } from '../days-counters.model';
import { chosenTypeDayoff } from '../dayoff.action';

export interface EventDialogState {
    dialogType: EventDialogType;
    chosenHoursCreditType: HoursCreditType; 
}

const initState: EventDialogState = {
    dialogType: null,
    chosenHoursCreditType: HoursCreditType.DaysOff
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
        default:
            return state;
    }
};