import { dataMember, required } from 'santee-dcts';
import { Moment } from 'moment';

export enum CalendarEventsType {
    Vacation = 1,
    SickLeave = 2,
    Dayoff = 3,
    AdditionalWork = 4
}

export enum CalendarEventStatus {
    Requested = 1,
    Approved = 2,
    Cancelled = 3
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