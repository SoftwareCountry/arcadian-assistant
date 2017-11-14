import { Reducer, combineReducers } from 'redux';
import { List } from 'immutable';
import { messagesReducer } from './messages.reducer';

export interface HelpdeskState {
    messageTemplates: {}[];
}

export const helpdeskReducer = combineReducers<HelpdeskState>({
    messageTemplates: messagesReducer,    
});