import React from 'react';
import { ActivityIndicator, Image, Platform, Text, TouchableOpacity, View } from 'react-native';
import { splashScreenStyles } from './styles';
import { FingerprintPopupAndroid } from '../fingerprint-popup/fingerprint-popup.android';
import { Action, Dispatch } from 'redux';
import { pinStore, startLoginProcess, startLogoutProcess } from '../reducers/auth/auth.action';
import { connect } from 'react-redux';
import FingerprintScanner from 'react-native-fingerprint-scanner';
import { FingerprintPopupIOS } from '../fingerprint-popup/fingerprint-popup.ios';
import { AuthState } from '../reducers/auth/auth.reducer';
import { AppState } from '../reducers/app.reducer';
import Style from '../layout/style';
import { welcomeScreenStyles } from '../welcome-screen/styles';
import PINCode from '@haskkor/react-native-pincode';

//============================================================================
interface SplashScreenState {
    fingerprintPopupVisible: boolean;
}

//============================================================================
interface SplashScreenDispatchProps {
    login: () => void;
    logout: () => void;
    storePin: (pin: string) => void;
    onLoginClicked: () => void;
}

//============================================================================
interface SplashScreenStateProps {
    authentication: AuthState | undefined;
}

//----------------------------------------------------------------------------
const Biometry = {
    iOS: {
        touchId: 'Touch ID',
        faceId: 'Face ID',
    },
    Android: {
        fingerprint: 'Fingerprint',
    },
};

//============================================================================
class SplashScreenImpl extends React.Component<SplashScreenStateProps & SplashScreenDispatchProps, SplashScreenState> {
    private isSensorAvailable = false;

    //----------------------------------------------------------------------------
    constructor(props: SplashScreenStateProps & SplashScreenDispatchProps) {
        super(props);

        this.state = {
            fingerprintPopupVisible: false,
        };
    }

    //----------------------------------------------------------------------------
    public render() {
        const authentication = this.props.authentication;

        if (authentication === undefined ||
            authentication.hasRefreshToken === undefined ||
            authentication.pinCode === undefined) {
            return (
                <View style={splashScreenStyles.container}>
                    <View style={splashScreenStyles.imageContainer}>
                        <Image source={require('./arcadia-logo.png')}/>
                    </View>
                </View>
            );
        }

        if (!authentication.hasRefreshToken) {
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

        if (!authentication.pinCode) {
            return (
                <View style={welcomeScreenStyles.container}>
                    <PINCode
                        stylePinCodeColorTitle={Style.color.white}
                        stylePinCodeColorSubtitle={Style.color.white}
                        status={'choose'}
                        storePin={(pin: string) => {
                            this.props.login();
                            this.props.storePin(pin);
                        }}/>
                </View>
            );
        }

        if (!this.isSensorAvailable) {
            return (
                <View style={welcomeScreenStyles.container}>
                    <PINCode
                        status={'enter'}
                        storedPin={authentication.pinCode}
                        touchIDDisabled={true}
                        disableLockScreen={true}
                        finishProcess={(_) => {
                            this.onSuccess();
                        }}
                        onFail={(attempts: number) => {
                            this.onFail(attempts);
                        }}/>
                </View>
            );
        }

        return this.fingerprintPopup();
    }

    //----------------------------------------------------------------------------
    public componentDidMount(): void {
        FingerprintScanner.isSensorAvailable()
            .then((biometry) => {
                this.handleBiometry(biometry);
            })
            .catch(() => {
                this.handleBiometry();
            });
    }

    //----------------------------------------------------------------------------
    private handleBiometry = (biometry?: string) => {
        const isSensorAvailable = !!biometry &&
            (biometry === Biometry.Android.fingerprint ||
                biometry === Biometry.iOS.touchId ||
                biometry === Biometry.iOS.faceId);

        this.isSensorAvailable = isSensorAvailable;

        if (!isSensorAvailable) {
            this.props.login();
        } else {
            this.setState({
                ...this.state,
                fingerprintPopupVisible: true,
            });
        }
    };

    //----------------------------------------------------------------------------
    private fingerprintPopup(): JSX.Element {

        const isIOS = Platform.OS === 'ios';
        if (isIOS) {
            return (
                <View style={welcomeScreenStyles.container}>
                    <PINCode
                        status={'enter'}
                        storedPin={this.props.authentication!.pinCode!}
                        touchIDDisabled={true}
                        disableLockScreen={true}
                        finishProcess={(_) => {
                            this.onSuccess();
                        }}
                        onFail={(attempts: number) => {
                            this.onFail(attempts);
                        }}/>
                    <FingerprintPopupIOS
                        onPopupHidden={this.onPopupHidden}/>
                </View>
            );
        } else {
            return (
                <View style={welcomeScreenStyles.container}>
                    <PINCode
                        status={'enter'}
                        storedPin={this.props.authentication!.pinCode!}
                        touchIDDisabled={true}
                        disableLockScreen={true}
                        finishProcess={(_) => {
                            this.onSuccess();
                        }}
                        onFail={(attempts: number) => {
                            this.onFail(attempts);
                        }}/>
                    <FingerprintPopupAndroid
                        isVisible={this.state.fingerprintPopupVisible}
                        onPopupClose={this.onPopupClosed}
                        onPopupHidden={this.onPopupHidden}/>
                </View>
            );
        }
    }

    //----------------------------------------------------------------------------
    private onPopupClosed = (): void => {
        this.setState({
            ...this.state,
            fingerprintPopupVisible: false,
        });
    };

    //----------------------------------------------------------------------------
    private onPopupHidden = (success: boolean): void => {
        if (success === undefined) {
            return;
        }

        if (success) {
            this.props.login();
        } else {
            //request pin
        }
    };

    //----------------------------------------------------------------------------
    private onSuccess = () => {
        this.props.login();
    };

    //----------------------------------------------------------------------------
    private onFail = (attempts: number) => {
        if (attempts > 3) {
            this.props.logout();
        }
    };
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState) => ({
    authentication: state.authentication,
});

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>) => {
    return {
        login: () => {
            dispatch(startLoginProcess());
        },
        logout: () => {
            dispatch(startLogoutProcess(true));
        },
        storePin: (pin: string) => {
            dispatch(pinStore(pin));
        },
        onLoginClicked: () => {
            dispatch(startLoginProcess());
        }
    };
};

export const SplashScreen = connect(stateToProps, dispatchToProps)(SplashScreenImpl);
