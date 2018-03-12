import { dataMember, required } from 'santee-dcts';
import { Moment } from 'moment';

export enum CalendarEventsType {
    Vacation = 'Vacation',
    Sickleave = 'Sickleave',
    Dayoff = 'Dayoff',
    Workout = 'Workoff'
}

export enum CalendarEventStatus {
    Requested = 'Requested',
    Approved = 'Approved',
    Cancelled = 'Cancelled'
}

export class DatesInterval {
    @dataMember()
    @required()
    public startDate: Moment;

    @dataMember()
    @required()
    public endDate: Moment;

    @dataMember()
    @required()
    public startWorkingHour: number;

    @dataMember()
    @required()
    public finishWorkingHour: number;
}

export class CalendarEvents {
    @dataMember()
    @required()
    public calendarEventId: string;

    @dataMember()
    @required()
    public type: CalendarEventsType;

    @dataMember()
    @required()
    public dates: DatesInterval;

    @dataMember()
    @required()
    public status: CalendarEventStatus;
}