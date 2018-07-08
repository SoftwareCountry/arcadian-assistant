import React, { Component } from 'react';
import { View, TouchableOpacity, Image } from 'react-native';
import { logoutStyles as styles } from './top-nav-bar-styles';
import { AuthActions, startLogoutProcess } from '../reducers/auth/auth.action';
import { connect, Dispatch } from 'react-redux';

interface LogoutDispatchProps {
    onLogoutClicked: () => void;
}
const mapDispatchToProps = (dispatch: Dispatch<AuthActions>): LogoutDispatchProps => ({
    onLogoutClicked: () => { dispatch(startLogoutProcess()); }
});

class LogoutViewImpl extends Component<LogoutDispatchProps> {
    public render() {
        return <TouchableOpacity onPress={this.props.onLogoutClicked}>
                    <View style={styles.logoutContainer}>
                        <Image style={styles.imageLogout} source={require('./logout-image.png')} />
                    </View>
                </TouchableOpacity>;
    }
}

export const LogoutView = connect(null, mapDispatchToProps)(LogoutViewImpl);