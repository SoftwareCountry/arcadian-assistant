import { Reducer } from 'redux';
import { TicketActions } from './tickets.actions';
import { TicketTemplate } from './ticket-template.model';

const tempMessage = new TicketTemplate();
tempMessage.templateId = 'call-saul';
tempMessage.title = 'Better Call Saul';
tempMessage.description = 'You know what to do';
tempMessage.recipients = [ 'IT Helpdesk' ];


const tempMessage2 = new TicketTemplate();
tempMessage.templateId = 'internet-is-dead';
tempMessage.title = 'Internet died';
tempMessage.description = 'again...';
tempMessage.recipients = [ 'IT Helpdesk' ];

export const ticketTemplatesReducer: Reducer<TicketTemplate[]> = (state = null, action: TicketActions) => {
    switch (action.type) {
        case 'LOAD-TICKET-TEMPLATES-FINISHED':
            return [...action.templates];

        default:
            return state;
    }
};