import { NavigationActions } from 'react-navigation';
import { Dispatch } from 'react-redux';

export interface WithBackButtonProps {
    onBackClick: () => void;
}

export const mapBackButtonDispatchToProps = (dispatch: Dispatch<any>) => ({
    onBackClick: () => dispatch(NavigationActions.back())
});