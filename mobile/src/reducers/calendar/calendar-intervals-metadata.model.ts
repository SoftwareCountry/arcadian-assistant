import { CalendarEvent } from './calendar-event.model';

export interface ReadOnlyIntervalsMetadata {
    readonly calendarEvents: ReadonlyArray<CalendarEvent>;

    copy(): IntervalsMetadata;
}

export class IntervalsMetadata implements ReadOnlyIntervalsMetadata {
    constructor(private readonly calendarEventsArray: CalendarEvent[]) {
    }

    public addCalendarEvent(...calendarEvents: CalendarEvent[]) {
        this.calendarEventsArray.push(...calendarEvents);
    }

    public get calendarEvents(): ReadonlyArray<CalendarEvent> {
        return this.calendarEventsArray;
    }

    public copy(): IntervalsMetadata {
        return new IntervalsMetadata([...this.calendarEventsArray]);
    }
}
