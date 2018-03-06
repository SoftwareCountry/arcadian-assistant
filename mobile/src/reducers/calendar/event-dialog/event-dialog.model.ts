import { Moment } from 'moment';
import { cancelDialog, CalendarActions } from '../calendar.action';
import { confirmSickLeave, editSickLeave, confirmProlongSickLeave, prolongSickLeave, completeSickLeave } from '../sick-leave.action';

export const eventDialogTextDateFormat = 'MMMM D, YYYY';

interface ButtonProps {
    label: string;
    action: () => CalendarActions;
}

export abstract class EventDialogModel<T extends EventDialogModel<T>> {
    public abstract title: string;
    public abstract text: string;
    public abstract icon: string;
    public cancel?: ButtonProps;
    public accept?: ButtonProps;
    public close?: () => CalendarActions;

    constructor(protected readonly tCtor: new () => T) {}

    public copy(): T {
        const instance = new this.tCtor();

        instance.title = this.title;
        instance.text = this.text;
        instance.icon = this.icon;
        instance.cancel = this.cancel ? {...this.cancel} : this.cancel;
        instance.accept = this.accept ? {...this.accept} : this.accept;
        instance.close = this.close;

        return instance;
    }
}

export class EventDialogEmptyModel extends EventDialogModel<EventDialogEmptyModel> {
    public title: string;
    public text: string;
    public icon: string;
    public cancel: ButtonProps = { label: null, action: null };
    public accept: ButtonProps = { label: null, action: null };

    constructor() {
        super(EventDialogEmptyModel);
    }
}

export class ClaimSickLeaveDialogModel extends EventDialogModel<ClaimSickLeaveDialogModel> {
    public startDate: Moment;
    public endDate: Moment;

    public readonly title = 'Select date to Complete your Sick Leave';

    public get text(): string {
        return `Your sick leave has started on ${this.startDate.format(eventDialogTextDateFormat)}${this.endDate ? ` and will be complete on ${this.endDate.format(eventDialogTextDateFormat)}` : ''}`;
    }

    public readonly icon = 'sick_leave';

    public readonly cancel: ButtonProps = {
        label: 'Back',
        action: cancelDialog
    };

    public readonly accept: ButtonProps = {
        label: 'Confirm',
        action: confirmSickLeave // TODO: add epic to confirmSickLeave
    };

    constructor() {
        super(ClaimSickLeaveDialogModel);
    }

    public copy(): ClaimSickLeaveDialogModel {
        const newInstance = super.copy();

        newInstance.startDate = this.startDate;
        newInstance.endDate = this.endDate;

        return newInstance;
    }
}

export class ProlongSickLeaveDialogModel extends EventDialogModel<ProlongSickLeaveDialogModel> {
    public readonly title = 'Select date to Prolong your sick leave';
    public readonly text = 'Your sick leave has started on MM D, YYYY and will be prolonged to MM D, YYYY.';
    public readonly icon = 'sick_leave';
    public readonly close = cancelDialog;
    public readonly cancel: ButtonProps = {
        label: 'Back',
        action: editSickLeave
    };
    public readonly accept: ButtonProps = {
        label: 'Confirm',
        action: confirmProlongSickLeave // TODO: add epic to confirmProlongSickLeave
    };

    constructor() {
        super(ProlongSickLeaveDialogModel);
    }
}

export class EditSickLeaveDialogModel extends EventDialogModel<EditSickLeaveDialogModel> {
    public readonly title = 'Hey! Hope you feel better';
    public readonly text = 'Your sick leave has started on MM D, YYYY and still not completed.';
    public readonly icon = 'sick_leave';
    public readonly close = cancelDialog;
    public readonly cancel: ButtonProps = {
        label: 'Prolong',
        action: prolongSickLeave
    };
    public readonly accept: ButtonProps = {
        label: 'Complete',
        action: completeSickLeave // TODO: add epic to completeSickLeave
    };

    constructor() {
        super(EditSickLeaveDialogModel);
    }
}