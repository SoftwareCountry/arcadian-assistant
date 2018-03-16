import { openEventDialog, closeEventDialog } from './event-dialog.action';
import { EventDialogType } from './event-dialog-type.model';
import { eventDialogReducer } from './event-dialog.reducer';

describe('event dialog reducer', () => {
    it('should return dialog type when open', () => {
        const eventTypeDialog = EventDialogType.ClaimSickLeave;
        const action = openEventDialog(eventTypeDialog);
        const state = eventDialogReducer(undefined, action);

        expect(state.dialogType).toBe(eventTypeDialog);
    });

    it('should return dialog type as null when close', () => {
        const action = closeEventDialog();
        const state = eventDialogReducer(undefined, action);

        expect(state.dialogType).toBeNull();
    });
});