import { Action } from 'redux';
import { TicketTemplate } from './ticket-template.model';

interface LoadTicketTemplates extends Action {
    type: 'LOAD-TICKET-TEMPLATES';
}

interface LoadTicketTemplatesFinished extends Action {
    type: 'LOAD-TICKET-TEMPLATES-FINISHED';
    templates: TicketTemplate[];
}

interface SubmitTicket extends Action {
    type: 'SUBMIT-TICKET';
    templateId: string;
    payload: string;
}

export type TicketActions = LoadTicketTemplates | LoadTicketTemplatesFinished | SubmitTicket;