/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/
import React, { Component } from 'react';
import { Image, View } from 'react-native';
import { BackNavigationArrowStyle } from './back-navigation-arrow.styles';

//============================================================================
export class BackNavigationArrow extends Component {

    //----------------------------------------------------------------------------
    public render() {
        return <View style={BackNavigationArrowStyle.container}>
            <Image style={BackNavigationArrowStyle.image}
                   source={require('../../../assets/icons/back/back-icon.png')}
                   resizeMethod={'resize'}/>
        </View>;
    }
}
