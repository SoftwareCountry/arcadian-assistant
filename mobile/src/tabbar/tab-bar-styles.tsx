import React from 'react';
import { StyleSheet } from 'react-native';

const tabBarStyles = StyleSheet.create({
    tabBar: {
        height: 59,
        backgroundColor: '#2FAFCC',
        justifyContent: 'center'
    },
    tabBarIndicator: {
        backgroundColor: 'transparent',
    },
    tabBarLabel: {
        color: '#FFFFFF',
        fontSize: 10,
        fontFamily: 'CenturyGothic',
        textAlign: 'center',
        marginBottom: 5,
    },
    tabImages: {
        color: '#FFFFFF',
    }
});


export default tabBarStyles;
