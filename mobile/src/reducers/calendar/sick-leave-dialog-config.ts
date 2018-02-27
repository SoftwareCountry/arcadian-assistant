import { DialogActiveState } from './calendar-events.reducer';
import { cancelDialog } from './calendar.action';
import { confirmSickLeave, editSickLeave, confirmProlongSickLeave, prolongSickLeave, completeSickLeave } from './sick-leave.action';

export const claimSickLeaveDialogConfig = (): DialogActiveState => (
    {
        active: true,
        title: 'Select date to Complete your Sick Leave',
        text: 'Your sick leave has started on March 5, 2018 and will be complete on March 14, 2018.',
        icon: 'sick_leave',
        cancel: {
            label: 'Back',
            action: cancelDialog
        },
        accept: {
            label: 'Confirm',
            action: confirmSickLeave // TODO: add epic to confirmSickLeave
        }
    }
);

export const prolongSickLeaveDialogConfig = (): DialogActiveState => (
    {
        active: true,
        title: 'Select date to Prolong your sick leave',
        text: 'Your sick leave has started on MM D, YYYY and will be prolonged to MM D, YYYY.',
        icon: 'sick_leave',
        close: cancelDialog,
        cancel: {
            label: 'Back',
            action: editSickLeave
        },
        accept: {
            label: 'Confirm',
            action: confirmProlongSickLeave // TODO: add epic to confirmProlongSickLeave
        }
    }
);

export const editSickLeaveDialogConfig = (): DialogActiveState => (
    {
        active: true,
        title: 'Hey! Hope you feel better',
        text: 'Your sick leave has started on MM D, YYYY and still not completed.',
        icon: 'sick_leave',
        close: cancelDialog,
        cancel: {
            label: 'Prolong',
            action: prolongSickLeave
        },
        accept: {
            label: 'Complete',
            action: completeSickLeave // TODO: add epic to completeSickLeave
        }
    }
);