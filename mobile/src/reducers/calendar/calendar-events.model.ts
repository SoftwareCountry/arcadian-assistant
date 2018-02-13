import { dataMember, required } from 'santee-dcts';
import { Moment } from 'moment';

export enum CalendarEventsType {
    Vacation = 'Vacation',
    SickLeave = 'SickLeave',
    Dayoff = 'SickLeave',
    AdditionalWork = 'SickLeave'
}

export enum CalendarEventStatus {
    Requested = 'Requested',
    Approved = 'Approved',
    Cancelled = 'Cancelled'
}

export class DatesPeriod {
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
    public dates: DatesPeriod;

    @dataMember()
    @required()
    public status: CalendarEventStatus;
}