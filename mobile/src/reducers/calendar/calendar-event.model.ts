import { dataMember, required, deserialize } from 'santee-dcts';
import moment, { Moment } from 'moment';

export enum CalendarEventType {
    Vacation = 'Vacation',
    Sickleave = 'Sickleave',
    Dayoff = 'Dayoff',
    Workout = 'Workout'
}

export enum CalendarEventStatus {
    Requested = 'Requested',
    Completed = 'Completed',
    Cancelled = 'Cancelled',
    Approved = 'Approved',
    Rejected = 'Rejected'
}

export class DatesInterval {
    @dataMember({
        customDeserializer: (value: string) => moment(value)
    })
    @required()
    public startDate: Moment;

    @dataMember({
        customDeserializer: (value: string) => moment(value)
    })
    @required()
    public endDate: Moment;

    @dataMember()
    @required()
    public startWorkingHour: number;

    @dataMember()
    @required()
    public finishWorkingHour: number;
}

export class CalendarEvent {
    @dataMember()
    @required()
    public calendarEventId: string;

    @dataMember()
    @required()
    public type: CalendarEventType;

    @dataMember({
        customDeserializer: (value: Object) => deserialize(value, DatesInterval)
    })
    @required()
    public dates: DatesInterval;

    @dataMember()
    @required()
    public status: CalendarEventStatus;

    public get isRequested(): boolean {
        return this.status === CalendarEventStatus.Requested;
    }

    public get isCompleted(): boolean {
        return this.status === CalendarEventStatus.Completed;
    }

    public get isCancelled(): boolean {
        return this.status === CalendarEventStatus.Cancelled;
    }

    public get isSickLeave(): boolean {
        return this.type === CalendarEventType.Sickleave;
    }

    public get isVacation(): boolean {
        return this.type === CalendarEventType.Vacation;
    }
}