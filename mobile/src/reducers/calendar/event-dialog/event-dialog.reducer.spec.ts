import { openEventDialog, closeEventDialog } from './event-dialog.action';
import { chosenTypeDayoff } from '../dayoff.action';
import { HoursCreditType } from '../days-counters.model';
import { EventDialogType } from './event-dialog-type.model';
import { eventDialogReducer } from './event-dialog.reducer';

describe('event dialog reducer', () => {
    it('should return dialog type when open', () => {
        const eventTypeDialog = EventDialogType.ClaimSickLeave;
        const action = openEventDialog(eventTypeDialog);
        const state = eventDialogReducer(undefined, action);

        expect(state.dialogType).toBe(eventTypeDialog);
    });

    it('should return dialog type as null and dayoff type as default when close', () => {
        const action = closeEventDialog();
        const state = eventDialogReducer(undefined, action);

        expect(state.dialogType).toBeNull();
        expect(state.chosenHoursCreditType).toBe(HoursCreditType.DaysOff);
    });

    it('should return dayoff type when chosen type', () => {
        const action = chosenTypeDayoff(true); // isWorkout
        const state = eventDialogReducer(undefined, action);

        expect(state.chosenHoursCreditType).toBe(HoursCreditType.Workout);
    });
});