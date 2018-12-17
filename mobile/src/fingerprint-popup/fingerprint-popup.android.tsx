import React, { Component } from 'react';
import FingerprintScanner from 'react-native-fingerprint-scanner';
import { FingerprintPopupStyle } from './fingerprint-popup.style';
import { Image, StyleProp, Text, TextStyle, TouchableOpacity, View, ViewStyle } from 'react-native';
import Modal from 'react-native-modal';

//============================================================================
interface FingerprintPopupState {
    error: boolean;
}

//============================================================================
interface FingerprintPopupProps {
    isVisible: boolean;
    onPopupClose: () => void;
    onPopupHidden: (success: boolean) => void;
}

//============================================================================
export class FingerprintPopupAndroid extends Component<FingerprintPopupProps, FingerprintPopupState> {
    private success = false;

    private fingerprintImage = require('./fingerprint_white_192x192.png');

    //----------------------------------------------------------------------------
    constructor(props: FingerprintPopupProps) {
        super(props);

        this.state = {
            error: false,
        };
    }

    //----------------------------------------------------------------------------
    public componentDidMount(): void {
        FingerprintScanner.authenticate({ onAttempt: this.handleAuthenticationAttempt })
            .then(() => {
                this.success = true;
                this.props.onPopupClose();
            })
            .catch((error) => {
                this.success = false;
                this.props.onPopupClose();
            });
    }

    //----------------------------------------------------------------------------
    public componentWillUnmount(): void {
        FingerprintScanner.release();
    }

    //----------------------------------------------------------------------------
    public render() {
        return (
            <Modal
                isVisible={this.props.isVisible}
                style={FingerprintPopupStyle.view.modal}
                onModalHide={this.onModalHide}>

                <View style={FingerprintPopupStyle.view.container}>

                    <Text style={FingerprintPopupStyle.text.title}>{'Sign in'}</Text>

                    <View
                        style={[FingerprintPopupStyle.view.fingerprintImageContainer, this.fingerprintContainerColor()]}>
                        <Image style={FingerprintPopupStyle.view.fingerprintImage}
                               source={this.fingerprintImage}/>
                    </View>

                    <Text
                        style={[FingerprintPopupStyle.text.description, this.descriptionColor()]}>{this.descriptionText()}</Text>

                    <TouchableOpacity style={FingerprintPopupStyle.view.button} onPress={this.onCancel}>
                        <Text style={FingerprintPopupStyle.text.buttonText}>
                            CANCEL
                        </Text>
                    </TouchableOpacity>

                </View>

            </Modal>
        );
    }

    //----------------------------------------------------------------------------
    private onCancel = () => {
        this.success = false;
        this.props.onPopupClose();
    };

    //----------------------------------------------------------------------------
    private onModalHide = () => {
        this.props.onPopupHidden(this.success);
    };

    //----------------------------------------------------------------------------
    private fingerprintContainerColor = (): StyleProp<ViewStyle> => {
        return {
            backgroundColor: this.state.error ? '#ea3d13' : '#2fafcc',
        };
    };

    //----------------------------------------------------------------------------
    private descriptionColor = (): StyleProp<TextStyle> => {
        return {
            color: this.state.error ? '#ea3d13' : '#a5a5a5',
        };
    };

    //----------------------------------------------------------------------------
    private descriptionText = (): string => {
        if (!this.state.error) {
            return 'Touch the fingerprint sensor';
        }

        return 'Not recognized';
    };

    //----------------------------------------------------------------------------
    private handleAuthenticationAttempt = (error: Error): void => {
        this.setState({
            error: !!error,
        });
    };
}
