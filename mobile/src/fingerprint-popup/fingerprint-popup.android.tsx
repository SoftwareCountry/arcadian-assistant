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

    //----------------------------------------------------------------------------
    constructor(props: FingerprintPopupProps) {
        super(props);

        this.handleAuthenticationAttempt = this.handleAuthenticationAttempt.bind(this);
        this.fingerprintContainerColor = this.fingerprintContainerColor.bind(this);
        this.descriptionColor = this.descriptionColor.bind(this);
        this.descriptionText = this.descriptionText.bind(this);

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
                style={{ justifyContent: 'flex-end', margin: 0 }}
                onModalHide={() => { this.props.onPopupHidden(this.success); }}>
                <View style={FingerprintPopupStyle.view.container}>
                    <Text style={FingerprintPopupStyle.text.title}>{'Sign in'}</Text>
                    <View style={[FingerprintPopupStyle.view.fingerImageContainer, this.fingerprintContainerColor()]}>
                        <Image style={FingerprintPopupStyle.view.fingerImage}
                               source={require('./fingerprint_white_192x192.png')}/>
                    </View>
                    <Text style={[FingerprintPopupStyle.text.description, this.descriptionColor()]}>{this.descriptionText()}</Text>
                    <TouchableOpacity style={FingerprintPopupStyle.view.button} onPress={() => {
                        this.success = false;
                        this.props.onPopupClose();
                    }}>
                        <Text style={FingerprintPopupStyle.text.buttonText}>
                            CANCEL
                        </Text>
                    </TouchableOpacity>
                </View>
            </Modal>
        );
    }

    //----------------------------------------------------------------------------
    private fingerprintContainerColor(): StyleProp<ViewStyle> {
        return {
            backgroundColor: this.state.error ? '#ea3d13' : '#2fafcc',
        };
    }

    //----------------------------------------------------------------------------
    private descriptionColor(): StyleProp<TextStyle> {
        return {
            color: this.state.error ? '#ea3d13' : '#a5a5a5',
        };
    }

    //----------------------------------------------------------------------------
    private descriptionText(): string {
        if (!this.state.error) {
            return 'Touch the fingerprint sensor';
        }

        return 'Not recognized';
    }

    //----------------------------------------------------------------------------
    private handleAuthenticationAttempt(error: Error): void {
        this.setState({
            error: !!error,
        });
    }
}
