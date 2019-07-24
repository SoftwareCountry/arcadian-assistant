import { combineReducers } from 'redux';
import { loadTicketTemplatesEpic$, ticketTemplatesReducer } from './ticket-templates.reducer';
import { TicketTemplate } from './ticket-template.model';
import { combineEpics } from 'redux-observable';

export interface HelpDeskState {
    ticketTemplates: TicketTemplate[];
}

export const helpDeskEpics = combineEpics(loadTicketTemplatesEpic$);

export const helpDeskReducer = combineReducers<HelpDeskState>({
    ticketTemplates: ticketTemplatesReducer,
});
