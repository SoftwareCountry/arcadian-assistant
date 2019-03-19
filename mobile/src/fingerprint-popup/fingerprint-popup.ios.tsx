/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React, { Component } from 'react';
import FingerprintScanner from 'react-native-fingerprint-scanner';

//============================================================================
interface FingerprintPopupProps {
    onPopupHidden: (success: boolean) => void;
}

//============================================================================
export class FingerprintPopupIOS extends Component<FingerprintPopupProps> {

    //----------------------------------------------------------------------------
    public componentDidMount(): void {
        FingerprintScanner.authenticate({
            description: 'Touch the fingerprint sensor',
            fallbackEnabled: false,
        })
            .then(() => {
                this.props.onPopupHidden(true);
            })
            .catch((error) => {
                this.props.onPopupHidden(false);
            });
    }

    //----------------------------------------------------------------------------
    public componentWillUnmount(): void {
        FingerprintScanner.release();
    }

    //----------------------------------------------------------------------------
    public render() {
        return null;
    }
}
