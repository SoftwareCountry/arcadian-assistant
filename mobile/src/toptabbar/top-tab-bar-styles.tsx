import React from 'react';
import { StyleSheet } from 'react-native';

const topTabBarStyles = StyleSheet.create({
    tabBar: {
        height: 59,
        backgroundColor: '#2FAFCC',
    },
    tabBarIndicator: {
        backgroundColor: 'transparent',
    },
    tabBarLabel: {
        color: '#FFFFFF',
        fontSize: 10,
        lineHeight: 12,
        textAlign: 'center',
        marginBottom: 5,
    },
    tabImages: {
        width: 30, 
        height: 26 
    }
});


export default topTabBarStyles;
