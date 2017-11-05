import { createStore, combineReducers, Reducer } from 'redux';
import { messagesReducer, MessageTemplateViewModel } from './messages.reducer';
import { List } from 'immutable';

export interface AppState {
    messages: List<MessageTemplateViewModel>;
}

const reducers = combineReducers({
    messages: messagesReducer,
});

export const store = createStore(reducers);