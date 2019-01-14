import React from 'react';
import { Image, Platform, View } from 'react-native';
import { splashScreenStyles } from './styles';
import { FingerprintPopupAndroid } from '../fingerprint-popup/fingerprint-popup.android';
import { Action, Dispatch } from 'redux';
import { startLoginProcess, startLogoutProcess } from '../reducers/auth/auth.action';
import { connect } from 'react-redux';
import FingerprintScanner from 'react-native-fingerprint-scanner';
import { FingerprintPopupIOS } from '../fingerprint-popup/fingerprint-popup.ios';

//============================================================================
interface SplashScreenState {
    fingerprintPopupVisible: boolean;
}

//============================================================================
interface SplashScreenDispatchProps {
    login: () => void;
    logout: () => void;
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
class SplashScreenImpl extends React.Component<SplashScreenDispatchProps, SplashScreenState> {
    private isSensorAvailable = false;

    //----------------------------------------------------------------------------
    constructor(props: SplashScreenDispatchProps) {
        super(props);

        this.state = {
            fingerprintPopupVisible: false,
        };
    }

    //----------------------------------------------------------------------------
    public render() {
        return (
            <View style={splashScreenStyles.container}>
                <View style={splashScreenStyles.imageContainer}>
                    <Image source={require('./arcadia-logo.png')}/>
                </View>
                {this.fingerprintPopup()}
            </View>
        );
    }

    //----------------------------------------------------------------------------
    public componentWillMount(): void {
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
    private fingerprintPopup(): JSX.Element | null {
        if (!this.isSensorAvailable) {
            return null;
        }

        const isIOS = Platform.OS === 'ios';
        if (isIOS) {
            return (
                <FingerprintPopupIOS
                    onPopupHidden={this.onPopupHidden}/>
            );
        } else {
            return (
                <FingerprintPopupAndroid
                    isVisible={this.state.fingerprintPopupVisible}
                    onPopupClose={this.onPopupClosed}
                    onPopupHidden={this.onPopupHidden}/>
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
            this.props.logout();
        }
    };
}

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>) => {
    return {
        login: () => {
            dispatch(startLoginProcess());
        },
        logout: () => {
            dispatch(startLogoutProcess(true));
        },
    };
};

export const SplashScreen = connect(undefined, dispatchToProps)(SplashScreenImpl);
