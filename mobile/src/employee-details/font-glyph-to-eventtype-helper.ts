import { Map } from 'immutable';

import { CalendarEventType } from '../reducers/calendar/calendar-event.model';

export const eventTypeToGlyphIcon: Map<string, string> = Map([
    [CalendarEventType.Dayoff, 'dayoff'],
    [CalendarEventType.Vacation, 'vacation'],
    [CalendarEventType.Sickleave, 'sick_leave'],
    [CalendarEventType.Workout, 'dayoff']
]);
