import React from 'react';
import { StyleSheet } from 'react-native';

//----------------------------------------------------------------------------
const color = {
    base: '#2FAFCC',
    white: '#FFFFFF',
};

//----------------------------------------------------------------------------
const view = StyleSheet.create({
    safeArea: {
        flex: 1,
        backgroundColor: color.white,
    },
    container: {
        flex: 1,
        backgroundColor: color.white,
        alignItems: 'center',
        justifyContent: 'center'
    },
});

//----------------------------------------------------------------------------
const navigation = StyleSheet.create({
    header: {
        backgroundColor: color.base,
    },
    title: {
        fontFamily: 'CenturyGothic',
        fontSize: 14,
        color: 'white',
        textAlign: 'center',
    },
});

//----------------------------------------------------------------------------
const Style = {
    view: {
        ...view,
    },

    navigation: {
        ...navigation,
    },

    color: {
        ...color,
    },
};

//----------------------------------------------------------------------------
export default Style;
