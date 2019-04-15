import React from 'react';
import { Image, Platform, Text, TouchableOpacity, View } from 'react-native';
import { splashScreenStyles } from './styles';
import { FingerprintPopupAndroid } from '../fingerprint-popup/fingerprint-popup.android';
import { Action, Dispatch } from 'redux';
import { pinStore, startLoginProcess, startLogoutProcess } from '../reducers/auth/auth.action';
import { connect } from 'react-redux';
import FingerprintScanner from 'react-native-fingerprint-scanner';
import { FingerprintPopupIOS } from '../fingerprint-popup/fingerprint-popup.ios';
import { AuthState } from '../reducers/auth/auth.reducer';
import { AppState } from '../reducers/app.reducer';
import ArcadiaPinCode from './pin-code';
import { PinResultStatus } from '@haskkor/react-native-pincode/dist/src/utils';

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
    public shouldComponentUpdate(nextProps: Readonly<SplashScreenStateProps & SplashScreenDispatchProps>, nextState: Readonly<SplashScreenState>, nextContext: any): boolean {
        if (this.state.fingerprintPopupVisible !== nextState.fingerprintPopupVisible) {
            return true;
        }

        const authentication = this.props.authentication;
        const nextAuthentication = nextProps.authentication;

        if (!authentication || !nextAuthentication) {
            return true;
        }

        if (authentication.hasRefreshToken !== nextAuthentication.hasRefreshToken) {
            return true;
        }

        if (authentication.pinCode !== nextAuthentication.pinCode) {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    public render() {

        const authentication = this.props.authentication;

        if (authentication === undefined ||
            authentication.hasRefreshToken === undefined ||
            authentication.pinCode === undefined) {
            return this.splash();
        }

        if (!authentication.hasRefreshToken) {
            return this.login();
        }

        if (!authentication.pinCode) {
            return this.setPinCode();
        }

        if (!this.isSensorAvailable) {
            return this.askPinCode(authentication.pinCode);
        }

        return this.askFingerprint(authentication.pinCode);
    }

    //----------------------------------------------------------------------------
    private handleBiometry = (biometry?: string) => {
        const isSensorAvailable = !!biometry &&
            (biometry === Biometry.Android.fingerprint ||
             biometry === Biometry.iOS.touchId ||
             biometry === Biometry.iOS.faceId);

        this.isSensorAvailable = isSensorAvailable;

        this.setState({
            ...this.state,
            fingerprintPopupVisible: isSensorAvailable,
        });
    };

    //----------------------------------------------------------------------------
    private splash(): JSX.Element {
        return (
            <View style={splashScreenStyles.container}>
                <View style={splashScreenStyles.imageContainer}>
                    <Image source={require('./arcadia-logo.png')}/>
                </View>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private login(): JSX.Element {
        return (
            <View style={splashScreenStyles.container}>
                <Text style={splashScreenStyles.greeting}>Welcome to Arcadia Assistant</Text>
                <View style={splashScreenStyles.loginButtonContainer}>
                    <TouchableOpacity onPress={this.props.onLoginClicked}>
                        <Text style={splashScreenStyles.loginText}>Login</Text>
                    </TouchableOpacity>
                </View>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private setPinCode(): JSX.Element {
        return (
            <View style={splashScreenStyles.container}>
                <ArcadiaPinCode
                    status={'choose'}
                    storePin={(pin: string) => {
                        this.pinStore(pin);
                        this.onSuccess();
                    }
                    }/>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private askPinCode(storedPin: string): JSX.Element {
        return (
            <View style={splashScreenStyles.container}>
                <ArcadiaPinCode
                    status={'enter'}
                    storedPin={storedPin}
                    finishProcess={this.onSuccess}
                    onClickButtonLockedPage={this.onLockedScreenClose}
                />
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private askFingerprint(storedPin: string): JSX.Element {

        const isIOS = Platform.OS === 'ios';

        const fingerprintPopup = isIOS ?
            <FingerprintPopupIOS
                onPopupHidden={this.onPopupHidden}/> :
            <FingerprintPopupAndroid
                isVisible={this.state.fingerprintPopupVisible}
                onPopupClose={this.onPopupClosed}
                onPopupHidden={this.onPopupHidden}/>;

        return (
            <View style={splashScreenStyles.container}>
                <ArcadiaPinCode
                    status={'enter'}
                    storedPin={storedPin}
                    finishProcess={this.onSuccess}
                    onClickButtonLockedPage={this.onLockedScreenClose}
                />
                {fingerprintPopup}
            </View>
        );
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
            this.onSuccess();
        }
    };

    //----------------------------------------------------------------------------
    private onSuccess = () => {
        this.props.login();
    };

    //----------------------------------------------------------------------------
    private onLockedScreenClose = () => {
        this.props.logout();
    };

    //----------------------------------------------------------------------------
    private pinStore = (pin: string) => {
        this.props.storePin(pin);
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
