import React from 'react';
import { Text, TouchableOpacity, View } from 'react-native';
import { connect } from 'react-redux';
import { AuthActions, startLoginProcess } from '../reducers/auth/auth.action';
import { welcomeScreenStyles } from './styles';
import { Dispatch } from 'redux';

interface AuthDispatchProps {
    onLoginClicked: () => void;
}

const mapDispatchToProps = (dispatch: Dispatch<AuthActions>): AuthDispatchProps => ({
    onLoginClicked: () => {
        dispatch(startLoginProcess());
    }
});

class WelcomeScreenImpl extends React.Component<AuthDispatchProps> {

    public render() {
        return (
            <View style={welcomeScreenStyles.container}>
                <Text style={welcomeScreenStyles.greeting}>Welcome to Arcadia Assistant</Text>
                <View style={welcomeScreenStyles.loginButtonContainer}>
                    <TouchableOpacity onPress={this.props.onLoginClicked}>
                        <Text style={welcomeScreenStyles.loginText}>Login</Text>
                    </TouchableOpacity>
                </View>

            </View>
        );
    }
}

export const WelcomeScreen = connect(null, mapDispatchToProps)(WelcomeScreenImpl);
