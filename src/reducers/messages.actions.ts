import { Action } from 'redux';

interface LoadTicketTemplates extends Action {
    type: 'LOAD-TICKET-TEMPLATES';
}

interface SubmitTicket extends Action {
    type: 'SUBMIT-TICKET';
    templateId: string;
    payload: string;
}

export type MessageActions = LoadTicketTemplates | SubmitTicket;