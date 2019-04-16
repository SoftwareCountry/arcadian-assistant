import React from 'react';
import { StyleSheet } from 'react-native';

//----------------------------------------------------------------------------
const color = {
    base: '#2FAFCC',
    white: '#FFFFFF',
    red: '#FF0000',
    black: '#000000',
    green: '#00FF00',
    transparent: 'transparent',
    pin: {
        yellow: '#F6CE46',
    },
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
        shadowOpacity: 0,
        borderBottomWidth: 0,
        elevation: 0,
    },
    titleContainer: {
        flex: 1,
        alignContent: 'center',
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
