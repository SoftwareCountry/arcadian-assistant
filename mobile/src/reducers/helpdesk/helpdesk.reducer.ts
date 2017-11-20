import { Reducer, combineReducers } from 'redux';
import { List } from 'immutable';
import { ticketTemplatesReducer, loadTicketTemplatesEpic$ } from './ticket-templates.reducer';
import { TicketTemplate } from './ticket-template.model';
import { Observable } from 'rxjs/Observable';
import { ActionsObservable, combineEpics } from 'redux-observable';
import { TicketAction } from './tickets.actions';

export interface HelpdeskState {
    ticketTemplates: TicketTemplate[];
}

export const helpdeskEpics = combineEpics( loadTicketTemplatesEpic$ );

export const helpdeskReducer = combineReducers<HelpdeskState>({
    ticketTemplates: ticketTemplatesReducer,
});