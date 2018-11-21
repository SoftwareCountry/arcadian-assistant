import React from 'react';
import { Image, Platform, View } from 'react-native';
import { splashScreenStyles } from './styles';
import { FingerprintPopupAndroid } from '../fingerprint-popup/fingerprint-popup.android';
import { Action } from 'redux';
import { startLoginProcess, startLogoutProcess } from '../reducers/auth/auth.action';
import { connect, Dispatch } from 'react-redux';
import FingerprintScanner from 'react-native-fingerprint-scanner';

//============================================================================
interface SplashScreenState {
    fingerprintPopupVisible: boolean;
}

//============================================================================
interface SplashScreenDispatchProps {
    login: () => void;
    logout: () => void;
}

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
        const isAndroid = Platform.OS === 'android';
        if (isAndroid) {
            FingerprintScanner.isSensorAvailable()
                .then((biometry) => {
                    const isSensorAvailable = biometry === 'Fingerprint';
                    this.handleSensorAvailability(isSensorAvailable);
                })
                .catch((_) => {
                    this.handleSensorAvailability(false);
                });
        }
    }

    //----------------------------------------------------------------------------
    private handleSensorAvailability = (isSensorAvailable: boolean) => {
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
        const isIOS = Platform.OS === 'ios';
        if (isIOS || !this.isSensorAvailable) {
            return null;
        }

        return (
            <FingerprintPopupAndroid
                isVisible={this.state.fingerprintPopupVisible}
                onPopupClose={this.onPopupClosed}
                onPopupHidden={this.onPopupHidden}/>
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
