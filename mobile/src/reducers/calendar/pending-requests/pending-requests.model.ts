import { dataMember, deserializeArray, required } from 'santee-dcts';
import { CalendarEvent } from '../calendar-event.model';
import { Map } from 'immutable';

export class PendingRequests {
    @dataMember({
        customDeserializer: (pendingRequests: any) => {
            let requests = Map<string, CalendarEvent[]>();
            Object.keys(pendingRequests).forEach(key => {
                const events = pendingRequests[key];
                requests = requests.set(key, deserializeArray(events as any, CalendarEvent));
            });
            return requests;
        }
    })
    @required()
    public events: Map<string, CalendarEvent[]> = Map<string, CalendarEvent[]>();
}
