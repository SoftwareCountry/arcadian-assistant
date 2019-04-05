/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { StyleSheet } from 'react-native';
import Style from '../../layout/style';

//----------------------------------------------------------------------------
export const BackNavigationArrowStyle = StyleSheet.create({
    container: {
        height: 35,
        width: 55,
    },
    image: {
        alignSelf: 'flex-start',
        marginLeft: 10,
        tintColor: Style.color.white,
    }
});
