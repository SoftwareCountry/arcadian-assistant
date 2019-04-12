import React from 'react';
import Style from '../layout/style';
import PINCode from '@haskkor/react-native-pincode';

/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

//============================================================================
interface PinCodeProps {
    status: 'choose' | 'enter' | 'locked';
    storedPin?: string;
    finishProcess?: () => void;
    onFail?: (attempts: number) => void;
    storePin?: (pin: string) => void;
}

//============================================================================
export default class ArcadiaPinCode extends React.Component<PinCodeProps> {
    public render() {
        return (
            <PINCode
                status={this.props.status}
                passwordLength={6}
                stylePinCodeColorTitle={Style.color.white}
                stylePinCodeColorSubtitle={Style.color.white}
                stylePinCodeColorTitleError={Style.color.pin.yellow}
                stylePinCodeColorSubtitleError={Style.color.pin.yellow}
                colorPasswordError={Style.color.pin.yellow}
                storedPin={this.props.storedPin}
                touchIDDisabled={true}
                disableLockScreen={true}
                finishProcess={this.props.finishProcess}
                onFail={this.props.onFail}
                storePin={this.props.storePin}
            />
        );
    }
}
