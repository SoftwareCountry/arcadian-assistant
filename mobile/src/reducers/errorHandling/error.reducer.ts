import { Reducer, combineReducers } from 'redux';
import { OrganizationActions } from '../organization/organization.action';
import { Alert } from 'react-native';

var showAlert: boolean = false;
function getAlert(message: string) {
    Alert.alert('Error', message);
}
export interface ErrorAlertState {
    isError: false;
}

export const errorReducer: Reducer<boolean> = (state = false, action: OrganizationActions) => {
    switch (action.type) {
        case 'ERROR-LOAD-FAILED':
            console.log('we shoud show alert4');
            getAlert("Server is not available");
        default:
            return state;
    }
};

export const errorAlertReducer = combineReducers<ErrorAlertState>({
    isError: errorReducer,
});