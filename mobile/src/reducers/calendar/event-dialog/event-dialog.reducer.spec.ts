import {
    closeEventDialog,
    openEventDialog,
    startEventDialogProgress,
    stopEventDialogProgress
} from './event-dialog.action';
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

    it('should return dialog type as null when close', () => {
        const action = closeEventDialog();
        const state = eventDialogReducer(undefined, action);

        expect(state.dialogType).toBeNull();
    });

    it('should return dayoff type as default when close', () => {
        const action = closeEventDialog();
        const state = eventDialogReducer(undefined, action);

        expect(state.chosenHoursCreditType).toBe(HoursCreditType.DaysOff);
    });

    it('should return dayoff type when chosen type', () => {
        const action = chosenTypeDayoff(true); // isWorkout
        const state = eventDialogReducer(undefined, action);

        expect(state.chosenHoursCreditType).toBe(HoursCreditType.Workout);
    });

    it('should return inProgress as true when start progress', () => {
        const action = startEventDialogProgress();
        const state = eventDialogReducer(undefined, action);

        expect(state.inProgress).toBeTruthy();
    });

    it('should return inProgress as false when stop progress', () => {
        const action = stopEventDialogProgress();
        const state = eventDialogReducer(undefined, action);

        expect(state.inProgress).toBeFalsy();
    });
});
