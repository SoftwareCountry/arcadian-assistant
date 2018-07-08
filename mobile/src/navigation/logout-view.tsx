import React, { Component } from 'react';
import { View, TouchableOpacity, Image } from 'react-native';
import { logoutStyles as styles } from './top-nav-bar-styles';

interface LogoutViewProps {
   onLogoutClicked: () => void;
}

export class LogoutView extends Component<LogoutViewProps> {
    public render() {
        return <TouchableOpacity onPress={this.props.onLogoutClicked}>
                    <View style={styles.logoutContainer}>
                        <Image style={styles.imageLogout} source={require('./logout-image.png')} />
                    </View>
                </TouchableOpacity>;
    }
}
