import React, { Component } from 'react';
import FingerprintScanner from 'react-native-fingerprint-scanner';
import { FingerprintPopupStyle } from './fingerprint-popup.style';
import { Image, Modal, StyleProp, Text, TextStyle, TouchableOpacity, View, ViewStyle } from 'react-native';

//============================================================================
interface FingerprintPopupState {
    error: boolean;
}

//============================================================================
interface FingerprintPopupProps {
    isVisible: boolean;
    onPopupClosed: (success: boolean) => void;
}

//============================================================================
export class FingerprintPopupAndroid extends Component<FingerprintPopupProps, FingerprintPopupState> {

    //----------------------------------------------------------------------------
    constructor(props: FingerprintPopupProps) {
        super(props);

        this.handleAuthenticationAttempt = this.handleAuthenticationAttempt.bind(this);
        this.fingerprintContainerColor = this.fingerprintContainerColor.bind(this);

        this.state = {
            error: false,
        };
    }

    //----------------------------------------------------------------------------
    public componentDidMount(): void {
        FingerprintScanner.authenticate({ onAttempt: this.handleAuthenticationAttempt })
            .then(() => {
                this.props.onPopupClosed(true);
            })
            .catch((error) => {
                this.props.onPopupClosed(false);
            });
    }

    //----------------------------------------------------------------------------
    public componentWillUnmount(): void {
        FingerprintScanner.release();
    }

    //----------------------------------------------------------------------------
    public render() {
        if (this.props.isVisible) {
            return (
                <Modal visible={this.props.isVisible} transparent={true} onRequestClose={() => {}}>
                    <View style={FingerprintPopupStyle.view.flexContainer}>
                        <View style={FingerprintPopupStyle.view.container}>
                            <Text style={FingerprintPopupStyle.text.title}>{'Sign in'}</Text>
                            <View style={[FingerprintPopupStyle.view.fingerImageContainer, this.fingerprintContainerColor()]}>
                                <Image style={FingerprintPopupStyle.view.fingerImage}
                                       source={require('./fingerprint_white_192x192.png')}/>
                            </View>
                            <Text style={[FingerprintPopupStyle.text.description, this.descriptionColor()]}>{this.descriptionText()}</Text>
                            <TouchableOpacity style={FingerprintPopupStyle.view.button} onPress={() => {
                                this.props.onPopupClosed(false);
                            }}>
                                <Text style={FingerprintPopupStyle.text.buttonText}>
                                    CANCEL
                                </Text>
                            </TouchableOpacity>
                        </View>
                    </View>
                </Modal>
            );
        } else {
            return null;
        }
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
