import React, { Component } from 'react';
import { Image, TouchableOpacity } from 'react-native';
import { AuthActions, startLogoutProcess } from '../reducers/auth/auth.action';
import { connect } from 'react-redux';
import { LogoutStyle } from './logout-view.styles';
import { Dispatch } from 'redux';

//============================================================================
interface LogoutDispatchProps {
    onLogoutClicked: () => void;
}

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<AuthActions>): LogoutDispatchProps => ({
    onLogoutClicked: () => {
        dispatch(startLogoutProcess());
    }
});

//============================================================================
class LogoutViewImpl extends Component<LogoutDispatchProps> {
    public render() {
        return <TouchableOpacity onPress={this.props.onLogoutClicked} style={LogoutStyle.container}>
            <Image style={LogoutStyle.image} source={require('../../assets/logout-image.png')} resizeMethod={'resize'}/>
        </TouchableOpacity>;
    }
}

//----------------------------------------------------------------------------
export const LogoutView = connect(null, dispatchToProps)(LogoutViewImpl);
