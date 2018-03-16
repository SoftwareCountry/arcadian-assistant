import { EventDialogActions } from './event-dialog.action';
import { EventDialogType } from './event-dialog/event-dialog-type.model';

export interface EventDialogState {
    dialogType: EventDialogType;
}

const initState: EventDialogState = {
    dialogType: null
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
                dialogType: null
            };
        }
        default:
            return state;
    }
};