import React, { Component } from 'react';
import { Image, TouchableOpacity, View } from 'react-native';
import { AuthActions, startLogoutProcess } from '../reducers/auth/auth.action';
import { connect, Dispatch } from 'react-redux';
import { LogoutStyle } from './logout-view.styles';

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
        return <TouchableOpacity onPress={this.props.onLogoutClicked}>
            <View style={LogoutStyle.container}>
                <Image style={LogoutStyle.image} source={require('./logout-image.png')}/>
            </View>
        </TouchableOpacity>;
    }
}

//----------------------------------------------------------------------------
export const LogoutView = connect(null, dispatchToProps)(LogoutViewImpl);
