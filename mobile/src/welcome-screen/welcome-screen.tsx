import React from 'react';
import {  View, Button, Image, Text } from 'react-native';
import { StyledText } from '../override/styled-text';
import { AppState } from '../reducers/app.reducer';
import { Dispatch, connect } from 'react-redux';
import { AuthActions, startLoginProcess } from '../reducers/auth/auth.action';
import { welcomeScreenStyles, welcomeScreenColor } from './styles';

interface AuthDispatchProps {
    onloginClicked: () => void;
}
const mapDispatchToProps = (dispatch: Dispatch<AuthActions>): AuthDispatchProps => ({
    onloginClicked: () => { dispatch(startLoginProcess()); }
});

class WelcomeScreenImpl extends React.Component<AuthDispatchProps> {

    public render() {
        return (
            <View style={welcomeScreenStyles.container}>
                <Text style={welcomeScreenStyles.greeting}>Welcome to Arcadia Assistant</Text>
                <View style={welcomeScreenStyles.loginbuttonContainer}>
                    <Button title='Login' onPress={this.props.onloginClicked} color={welcomeScreenColor} />
                </View>

            </View>
        );
    }
}
export const WelcomeScreen = connect(null, mapDispatchToProps)(WelcomeScreenImpl);
