import { dataMember, deserialize, required } from 'santee-dcts';
import moment, { Moment } from 'moment';

export enum CalendarEventType {
    Vacation = 'Vacation',
    Sickleave = 'Sickleave',
    Dayoff = 'Dayoff',
    Workout = 'Workout',
}

export enum GeneralCalendarEventStatus {
    Requested = 'Requested',
    Cancelled = 'Cancelled',
    Approved = 'Approved',
    Rejected = 'Rejected',
}

export enum VacationStatus {
    Requested = 'Requested',
    Cancelled = 'Cancelled',
    Approved = 'Approved',
    Rejected = 'Rejected',
    AccountingReady = 'AccountingReady',
    Processed = 'Processed',
}

export enum SickleaveStatus {
    Requested = 'Requested',
    Cancelled = 'Cancelled',
    Completed = 'Completed',
}

export enum DayoffWorkoutStatus {
    Requested = 'Requested',
    Cancelled = 'Cancelled',
    Approved = 'Approved',
    Rejected = 'Rejected',
}

export type CalendarEventStatus = GeneralCalendarEventStatus | VacationStatus | SickleaveStatus | DayoffWorkoutStatus;

export class DatesInterval {
    private static dateFormat = 'MM/DD/YYYY';

    @dataMember({
        customDeserializer: (value: string) => moment(value)
    })
    @required()
    public startDate: Moment = moment();

    @dataMember({
        customDeserializer: (value: string) => moment(value)
    })
    @required()
    public endDate: Moment = moment();

    @dataMember()
    @required()
    public startWorkingHour: number = 0;

    @dataMember()
    @required()
    public finishWorkingHour: number = 8;

    public toJSON(): Object {
        const overridden: { [key in keyof DatesInterval]?: any } = {
            startDate: moment.isMoment(this.startDate) ? this.startDate.format(DatesInterval.dateFormat) : null,
            endDate: moment.isMoment(this.endDate) ? this.endDate.format(DatesInterval.dateFormat) : null,
        };

        const serialized = Object.assign({}, this, overridden);

        return serialized;
    }
}

export type CalendarEventId = string;

export class CalendarEvent {
    @dataMember()
    @required()
    public calendarEventId = '';

    @dataMember()
    @required()
    public type: CalendarEventType = CalendarEventType.Vacation;

    @dataMember({
        customDeserializer: (value: Object) => deserialize(value, DatesInterval)
    })
    @required()
    public dates: DatesInterval = new DatesInterval();

    @dataMember()
    @required()
    public status: CalendarEventStatus = GeneralCalendarEventStatus.Requested;

    public get isRequested(): boolean {
        return this.status === GeneralCalendarEventStatus.Requested;
    }

    public get isCompleted(): boolean {
        return this.type === CalendarEventType.Sickleave && this.status === SickleaveStatus.Completed;
    }

    public get isCancelled(): boolean {
        return this.status === GeneralCalendarEventStatus.Cancelled;
    }

    public get isApproved(): boolean {
        if (this.type === CalendarEventType.Sickleave) {
            return this.status === SickleaveStatus.Requested;
        }

        return this.status === GeneralCalendarEventStatus.Approved;
    }

    public get isAccountingReady(): boolean {
        return this.type === CalendarEventType.Vacation && this.status === VacationStatus.AccountingReady;
    }

    public get isProcessed(): boolean {
        return this.type === CalendarEventType.Vacation && this.status === VacationStatus.Processed;
    }

    public get isSickLeave(): boolean {
        return this.type === CalendarEventType.Sickleave;
    }

    public get isVacation(): boolean {
        return this.type === CalendarEventType.Vacation;
    }

    public get isWorkout(): boolean {
        return this.type === CalendarEventType.Workout;
    }

    public get isDayoff(): boolean {
        return this.type === CalendarEventType.Dayoff;
    }
}
