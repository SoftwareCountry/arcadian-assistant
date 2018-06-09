import { dataMember, required, deserialize } from 'santee-dcts';
import moment, { Moment } from 'moment';
import { Map } from 'immutable';
import { IntervalTypeConverter } from './interval-type-converter';
import { IntervalType } from './calendar.model';

export enum CalendarEventType {
    Vacation = 'Vacation',
    Sickleave = 'Sickleave',
    Dayoff = 'Dayoff',
    Workout = 'Workout'
}

export const eventTypeToGlyphIcon: Map<string, string> = Map([
    [CalendarEventType.Dayoff, 'dayoff'],
    [CalendarEventType.Vacation, 'vacation'],
    [CalendarEventType.Sickleave, 'sick_leave'],
    [CalendarEventType.Workout, 'dayoff']
]);

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

    public get isApproved(): boolean {
        return this.status === CalendarEventStatus.Approved;
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

    public get whichHalf(): string {
        let halfDescription: string;

        const intervalType = IntervalTypeConverter.hoursToIntervalType(this.dates.startWorkingHour, this.dates.finishWorkingHour);

        switch (intervalType) {
            case IntervalType.IntervalLeftBoundary:
                halfDescription = ' (first half)';
                break;
            case IntervalType.IntervalRightBoundary:
                halfDescription = ' (second half)';
                break;
            case IntervalType.IntervalFullBoundary:
                halfDescription = ' (whole day)';
                break;
        }

        return halfDescription;
    }

    public get descriptionFromTo(): string {
        let description: string;
        const eventDigitsDateFormat = 'DD/MM/YYYY';

        if (this.isWorkout || this.isDayoff) {
            description = 'on ' + this.dates.startDate.format(eventDigitsDateFormat) + this.whichHalf;
        } else {
            description = 'from ' + this.dates.startDate.format(eventDigitsDateFormat) + ' to ' + this.dates.endDate.format(eventDigitsDateFormat);
        }

        return description;
    }

    public get descriptionStatus(): string {
        let description: string;

        if (this.isRequested) {
            description = 'requests ' + this.type.toLowerCase();
        } else if (this.isApproved) {
            let prefix = this.dates.endDate.isAfter(moment(), 'date') ? 'has coming ' : 'on ';
            description = prefix + this.type.toLowerCase();
        }

        return description;
    }
}