import { Moment } from 'moment';
import { cancelDialog, CalendarActions } from '../calendar.action';
import { confirmSickLeave, editSickLeave, confirmProlongSickLeave, prolongSickLeave, completeSickLeave, confirmStartDateSickLeave, backToClaimSickLeave } from '../sick-leave.action';
import { Employee } from '../../organization/employee.model';
import { CalendarEvents } from '../calendar-events.model';

export const eventDialogTextDateFormat = 'MMMM D, YYYY';

export interface EventDialogActionArgs {
    userEmployee: Employee;
}

export abstract class EventDialogModel<T extends EventDialogModel<T>> {
    public abstract title: string;
    public abstract text: string;
    public abstract icon: string;

    public abstract cancelLabel: string;
    public abstract acceptLabel: string;

    public close?: () => CalendarActions;

    constructor(protected readonly tCtor: new () => T) {}

    public abstract cancelAction(): CalendarActions;
    public abstract acceptAction(args: EventDialogActionArgs): CalendarActions;

    public disableAccept(): boolean {
        return false;
    }

    public copy(): T {
        const instance = new this.tCtor();

        instance.title = this.title;
        instance.text = this.text;
        instance.icon = this.icon;
        instance.cancelLabel = this.cancelLabel;
        instance.acceptLabel = this.acceptLabel;
        instance.close = this.close;

        return instance;
    }
}

export class EventDialogEmptyModel extends EventDialogModel<EventDialogEmptyModel> {
    public title: string;
    public text: string;
    public icon: string;

    public cancelLabel: string;
    public acceptLabel: string;

    constructor() {
        super(EventDialogEmptyModel);
    }

    public cancelAction(): CalendarActions {
        return cancelDialog();
    }
    public acceptAction(): CalendarActions {
        return cancelDialog();
    }
}

export class ClaimSickLeaveDialogModel extends EventDialogModel<ClaimSickLeaveDialogModel> {
    public readonly title = 'Select date to Start your Sick Leave';
    public readonly icon = 'sick_leave';
    public readonly cancelLabel = 'Back';
    public readonly acceptLabel = 'Confirm';

    public startDate: Moment;

    constructor() {
        super(ClaimSickLeaveDialogModel);
    }

    public get text(): string {
        return `Your sick leave starts on ${this.startDate.format(eventDialogTextDateFormat)}`;
    }

    public cancelAction(): CalendarActions {
        return cancelDialog();
    }

    public acceptAction(args: EventDialogActionArgs): CalendarActions {
        return confirmStartDateSickLeave(this.startDate);
    }

    public copy(): ClaimSickLeaveDialogModel {
        const newInstance = super.copy();

        newInstance.startDate = this.startDate;

        return newInstance;
    }
}

export class SelectEndDateSickLeaveDialogModel extends EventDialogModel<SelectEndDateSickLeaveDialogModel> {
    public startDate: Moment;
    public endDate: Moment;
    public calendarEvents: CalendarEvents;

    public readonly title = 'Select date to Complete your Sick Leave';

    public get text(): string {
        return `Your sick leave has started on ${this.startDate.format(eventDialogTextDateFormat)}${this.endDate ? ` and will be complete on ${this.endDate.format(eventDialogTextDateFormat)}` : ''}`;
    }

    public readonly icon = 'sick_leave';

    public readonly cancelLabel = 'Back';
    public readonly acceptLabel = 'Confirm';

    constructor() {
        super(SelectEndDateSickLeaveDialogModel);
    }

    public cancelAction(): CalendarActions {
        return backToClaimSickLeave(this.startDate);
    }

    public acceptAction(args: EventDialogActionArgs): CalendarActions {
        return confirmSickLeave(args.userEmployee, this.calendarEvents);
    }

    public disableAccept() {
        return !this.calendarEvents;
    }

    public close = () => cancelDialog();

    public copy(): SelectEndDateSickLeaveDialogModel {
        const newInstance = super.copy();

        newInstance.startDate = this.startDate;
        newInstance.endDate = this.endDate;
        newInstance.calendarEvents = this.calendarEvents;

        return newInstance;
    }
}

export class ProlongSickLeaveDialogModel extends EventDialogModel<ProlongSickLeaveDialogModel> {
    public readonly title = 'Select date to Prolong your sick leave';
    public readonly text = 'Your sick leave has started on MM D, YYYY and will be prolonged to MM D, YYYY.';
    public readonly icon = 'sick_leave';
    public readonly close = cancelDialog;

    public readonly cancelLabel = 'Back';
    public readonly acceptLabel = 'Confirm';

    constructor() {
        super(ProlongSickLeaveDialogModel);
    }

    public cancelAction(): CalendarActions {
        return editSickLeave();
    }

    public acceptAction(): CalendarActions {
        return confirmProlongSickLeave(); // TODO: add epic to confirmProlongSickLeave
    }
}

export class EditSickLeaveDialogModel extends EventDialogModel<EditSickLeaveDialogModel> {
    public readonly title = 'Hey! Hope you feel better';
    public readonly text = 'Your sick leave has started on MM D, YYYY and still not completed.';
    public readonly icon = 'sick_leave';
    public readonly close = cancelDialog;

    public readonly cancelLabel = 'Prolong';
    public readonly acceptLabel = 'Complete';

    constructor() {
        super(EditSickLeaveDialogModel);
    }

    public cancelAction(): CalendarActions {
        return prolongSickLeave();
    }

    public acceptAction(): CalendarActions {
        return completeSickLeave(); // TODO: add epic to completeSickLeave
    }
}