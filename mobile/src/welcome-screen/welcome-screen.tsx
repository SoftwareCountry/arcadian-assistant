import React from 'react';
import { Button, Text, View } from 'react-native';
import { connect } from 'react-redux';
import { AuthActions, startLoginProcess } from '../reducers/auth/auth.action';
import { welcomeScreenColor, welcomeScreenStyles } from './styles';
import { Dispatch } from 'redux';

interface AuthDispatchProps {
    onloginClicked: () => void;
}

const mapDispatchToProps = (dispatch: Dispatch<AuthActions>): AuthDispatchProps => ({
    onloginClicked: () => {
        dispatch(startLoginProcess());
    }
});

class WelcomeScreenImpl extends React.Component<AuthDispatchProps> {

    public render() {
        return (
            <View style={welcomeScreenStyles.container}>
                <Text style={welcomeScreenStyles.greeting}>Welcome to Arcadia Assistant</Text>
                <View style={welcomeScreenStyles.loginButtonContainer}>
                    <Button title='Login' onPress={this.props.onloginClicked} color={welcomeScreenColor}/>
                </View>

            </View>
        );
    }
}

export const WelcomeScreen = connect(null, mapDispatchToProps)(WelcomeScreenImpl);
