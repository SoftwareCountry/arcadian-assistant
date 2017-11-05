import { Reducer } from 'redux';
import { List } from 'immutable';
import { Types } from './messages.actions';

export class MessageTemplate {
    public id: string;
    public title: string;
    public text: string;
}

export interface MessageTemplateViewModel {
    readonly message: MessageTemplate;
    readonly isExpanded: boolean;
}

const tempMessage = new MessageTemplate();
tempMessage.id = 'call-saul';
tempMessage.title = 'Better Call Saul';
tempMessage.text = 'You know what to do';


const tempMessage2 = new MessageTemplate();
tempMessage.id = 'internet-is-dead';
tempMessage.title = 'Internet died';
tempMessage.text = 'again...';

export const messagesReducer: Reducer<List<MessageTemplateViewModel>> = (state = List.of(), action) => {
    switch (action.type) {
        case Types.LOADLIST:
            return List.of(
                { message: tempMessage, isExpanded: false },
                { message: tempMessage2, isExpanded: false }
             );
        case Types.COLLAPSE:
             return state.map(x => x.message.id === action.id ? { message: tempMessage, isExpanded: false } : x ).toList();
        case Types.EXPAND:
             return state.map(x => x.message.id === action.id ? { message: tempMessage, isExpanded: true } : x ).toList();

        case Types.SEND:
        default:
            return state;
    }
};