import React from 'react';
import { StyleSheet } from 'react-native';
import Style from '../layout/style';

const tabBarStyles = StyleSheet.create({
    tabBar: {
        height: 59,
        backgroundColor: Style.color.base,
        justifyContent: 'center'
    },
    tabBarIndicator: {
        backgroundColor: 'transparent',
    },
    tabBarLabel: {
        color: Style.color.white,
        fontSize: 10,
        fontFamily: 'CenturyGothic',
        textAlign: 'center',
        marginBottom: 5,
    },
    tabImages: {
        color: Style.color.white,
    }
});


export default tabBarStyles;
