import { Action } from 'redux';
import { TicketTemplate } from './ticket-template.model';

export interface LoadTicketTemplates extends Action {
    type: 'LOAD-TICKET-TEMPLATES';
}

export const loadTicketTemplates = (): LoadTicketTemplates => ({ type: 'LOAD-TICKET-TEMPLATES' });

export interface LoadTicketTemplatesFinished extends Action {
    type: 'LOAD-TICKET-TEMPLATES-FINISHED';
    templates: TicketTemplate[];
}

export const loadTicketTemplatesFinished = (templates: TicketTemplate[]): LoadTicketTemplatesFinished =>
    ({ type: 'LOAD-TICKET-TEMPLATES-FINISHED', templates });

export interface SubmitTicket extends Action {
    type: 'SUBMIT-TICKET';
    templateId: string;
    payload: string;
}

export type TicketAction = LoadTicketTemplates | LoadTicketTemplatesFinished | SubmitTicket;