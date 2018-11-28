import { NavigationActions } from 'react-navigation';
import {Action, Dispatch} from 'redux';

export interface WithBackButtonProps {
    onBackClick: () => void;
}

export const mapBackButtonDispatchToProps = (dispatch: Dispatch<Action>) => ({
    onBackClick: () => dispatch(NavigationActions.back())
});
