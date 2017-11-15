import { Reducer, combineReducers } from 'redux';
import { List } from 'immutable';
import { ticketTemplatesReducer } from './ticket-templates.reducer';
import { TicketTemplate } from './ticket-template.model';

export interface HelpdeskState {
    ticketTemplates: TicketTemplate[];
}

export const helpdeskReducer = combineReducers<HelpdeskState>({
    ticketTemplates: ticketTemplatesReducer,    
});