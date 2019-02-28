import { dataMember, deserialize, required } from 'santee-dcts';
import moment, { Moment } from 'moment';

export enum CalendarEventType {
    Vacation = 'Vacation',
    Sickleave = 'Sickleave',
    Dayoff = 'Dayoff',
    Workout = 'Workout',
}

export enum CalendarEventStatus {
    Requested = 'Requested',
    Completed = 'Completed',
    Cancelled = 'Cancelled',
    Approved = 'Approved',
    Rejected = 'Rejected',
    Processed = 'Processed',
}

export class DatesInterval {
    private static dateFormat = 'L';

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
    public finishWorkingHour: number = 0;

    public toJSON(): Object {
        const overrided: { [key in keyof DatesInterval]?: any } = {
            startDate: moment.isMoment(this.startDate) ? this.startDate.format(DatesInterval.dateFormat) : null,
            endDate: moment.isMoment(this.endDate) ? this.endDate.format(DatesInterval.dateFormat) : null,
        };

        const serialized = Object.assign({}, this, overrided);

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
    public status: CalendarEventStatus = CalendarEventStatus.Requested;

    public get isRequested(): boolean {
        return this.status === CalendarEventStatus.Requested;
    }

    public get isCompleted(): boolean {
        return this.status === CalendarEventStatus.Completed;
    }

    public get isCancelled(): boolean {
        return this.status === CalendarEventStatus.Cancelled;
    }

    public get isApproved(): boolean {
        return this.status === CalendarEventStatus.Approved;
    }

    public get isProcessed(): boolean {
        return this.status === CalendarEventStatus.Processed;
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
