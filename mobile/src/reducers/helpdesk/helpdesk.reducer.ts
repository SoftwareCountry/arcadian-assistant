import { combineReducers } from 'redux';
import { loadTicketTemplatesEpic$, ticketTemplatesReducer } from './ticket-templates.reducer';
import { TicketTemplate } from './ticket-template.model';
import { combineEpics } from 'redux-observable';

export interface HelpdeskState {
    ticketTemplates: TicketTemplate[];
}

export const helpdeskEpics = combineEpics(loadTicketTemplatesEpic$);

export const helpdeskReducer = combineReducers<HelpdeskState>({
    ticketTemplates: ticketTemplatesReducer,
});
