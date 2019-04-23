/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React from 'react';
import Style from '../layout/style';
import PINCode, { resetPinCodeInternalStates } from '@haskkor/react-native-pincode';
import AsyncStorage from '@react-native-community/async-storage';
import { Image, Text, TouchableOpacity, View } from 'react-native';
import { pinCodeStyles } from './styles';

//----------------------------------------------------------------------------
const AsyncStorageKey = {
    pinLocked: 'pinLocked',
};

export declare type PinCodeStatus = 'choose' | 'enter' | 'locked';

//============================================================================
interface PinCodeProps {
    status: PinCodeStatus;
    storedPin?: string;
    finishProcess?: () => void;
    onFail?: (attempts: number) => void;
    storePin?: (pin: string) => void;
    useLogoutButton: boolean;
    onClickLogoutButton?: () => void;
}

//============================================================================
export default class ArcadiaPinCode extends React.Component<PinCodeProps> {

    //----------------------------------------------------------------------------
    public static isLocked(): Promise<boolean> {
        return AsyncStorage.getItem(AsyncStorageKey.pinLocked).then(
            (value) => { return !!value; },
            (_) => { return false; }
        );
    }

    //----------------------------------------------------------------------------
    public render() {
        return (
            <View style={pinCodeStyles.container}>
                <PINCode
                    status={this.props.status}
                    textButtonLockedPage={'Logout'}
                    passwordLength={6}
                    stylePinCodeColorTitle={Style.color.black}
                    stylePinCodeColorSubtitle={Style.color.black}
                    stylePinCodeColorTitleError={Style.color.red}
                    stylePinCodeColorSubtitleError={Style.color.red}
                    colorPassword={Style.color.base}
                    colorPasswordEmpty={Style.color.base}
                    colorPasswordError={Style.color.red}
                    numbersButtonOverlayColor={Style.color.base}
                    stylePinCodeTextTitle={pinCodeStyles.stylePinCodeTextTitle}
                    stylePinCodeTextSubtitle={pinCodeStyles.stylePinCodeTextSubtitle}
                    styleLockScreenMainContainer={pinCodeStyles.styleLockScreenMainContainer}
                    styleLockScreenViewTextLock={pinCodeStyles.styleLockScreenViewTextLock}
                    styleLockScreenTitle={pinCodeStyles.styleLockScreenTitle}
                    styleLockScreenViewCloseButton={pinCodeStyles.styleLockScreenViewCloseButton}
                    styleLockScreenButton={pinCodeStyles.styleLockScreenButton}
                    styleLockScreenTextButton={pinCodeStyles.styleLockScreenTextButton}
                    styleLockScreenText={pinCodeStyles.styleLockScreenText}
                    styleLockScreenViewIcon={pinCodeStyles.styleLockScreenViewIcon}
                    storedPin={this.props.storedPin}
                    touchIDDisabled={true}
                    disableLockScreen={false}
                    finishProcess={this.props.finishProcess}
                    onFail={this.props.onFail}
                    storePin={this.props.storePin}
                    onClickButtonLockedPage={this.onClickLogoutButton}
                    timePinLockedAsyncStorageName={AsyncStorageKey.pinLocked}
                    bottomLeftComponent={this.logoutButton()}
                />
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private logoutButton = (): any => {
        if (!this.props.useLogoutButton) {
            return null;
        }

        return (
            <TouchableOpacity onPress={this.onClickLogoutButton}>
                <Image style={pinCodeStyles.styleLockScreenLogoutImage}
                       source={require('../../assets/logout-image.png')} resizeMethod={'resize'}/>
                <Text style={pinCodeStyles.styleLockScreenLogoutButtonText}>Log out</Text>
            </TouchableOpacity>
        );
    };

    //----------------------------------------------------------------------------
    private onClickLogoutButton = (): void => {
        // noinspection JSIgnoredPromiseFromCall
        resetPinCodeInternalStates(undefined, AsyncStorageKey.pinLocked);

        if (this.props.onClickLogoutButton) {
            this.props.onClickLogoutButton();
        }
    };
}
