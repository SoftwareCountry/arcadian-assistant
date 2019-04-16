/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React from 'react';
import Style from '../layout/style';
import PINCode from '@haskkor/react-native-pincode';
import AsyncStorage from '@react-native-community/async-storage';
import { colors } from '@haskkor/react-native-pincode/dist/src/design/colors';
import { View } from 'react-native';
import { grid } from '@haskkor/react-native-pincode/src/design/grid';
import { pinCodeStyles } from './styles';

//----------------------------------------------------------------------------
const AsyncStorageKey = {
    pinLocked: 'pinLocked',
    pinAttempts: 'pinAttempts',
};

//============================================================================
interface PinCodeProps {
    status: 'choose' | 'enter' | 'locked';
    storedPin?: string;
    finishProcess?: () => void;
    onFail?: (attempts: number) => void;
    storePin?: (pin: string) => void;
    onClickButtonLockedPage?: () => void;
}

//============================================================================
export default class ArcadiaPinCode extends React.Component<PinCodeProps> {
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
                    onClickButtonLockedPage={() => {

                        // noinspection JSIgnoredPromiseFromCall
                        AsyncStorage.multiRemove([
                            AsyncStorageKey.pinLocked,
                            AsyncStorageKey.pinAttempts,
                        ]);

                        if (this.props.onClickButtonLockedPage) {
                            this.props.onClickButtonLockedPage();
                        }
                    }}
                    timePinLockedAsyncStorageName={AsyncStorageKey.pinLocked}
                    pinAttemptsAsyncStorageName={AsyncStorageKey.pinAttempts}
                />
            </View>
        );
    }
}
